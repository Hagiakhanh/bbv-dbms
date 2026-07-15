# BufferPoolManager — Internal Operations

Context: The `BufferPoolManager` (BPM) is the central memory manager of the storage engine. The [page-fetching.md](overview/page-fetching.md) overview shows the high-level flow; this diagram zooms into the **complete BPM operation set**: `FetchPage`, `UnpinPage`, `FlushPage`, `FlushAllPages`, `NewPage`, and `DeletePage`. Understanding the internal `BufferFrame` state machine is essential for writing `BufferPoolManagerTests`.

---

## Part A — BufferFrame State Machine

Each slot in the buffer pool is a `BufferFrame`:

```
BufferFrame states:
                    ┌──────────────┐
     ──────────────►│    FREE      │◄─────────────────────────────────────
     (initial /     │ PageId = -1  │                                      │
      after delete) │ PinCount = 0 │                                      │
                    │ IsDirty=false│                                      │
                    └──────┬───────┘                                      │
                           │ FetchPage(pageId) / NewPage()                │
                           │ ReadPage from disk → load data               │
                           ▼                                              │
                    ┌──────────────┐   UnpinPage(dirty=true)  ┌──────────────────┐
                    │   PINNED     │─────────────────────────►│ PINNED + DIRTY   │
                    │ PinCount ≥ 1 │◄─────────────────────────│ PinCount ≥ 1    │
                    │ IsDirty=false│  UnpinPage(dirty=false)   └──────────────────┘
                    └──────┬───────┘                                  │
          UnpinPage        │ (PinCount drops to 0)                    │ (PinCount drops to 0)
          → Replacer.Unpin ▼                                          ▼
                    ┌──────────────┐                     ┌──────────────────┐
                    │  EVICTABLE   │                     │ EVICTABLE+DIRTY  │
                    │ PinCount = 0 │    FlushPage()      │ PinCount = 0     │
                    │ IsDirty=false│◄────────────────────│ IsDirty = true   │
                    └──────┬───────┘                     └──────────────────┘
                           │ Replacer.Victim() picks this frame
                           │ (no WritePage needed — clean)
                           └──────────────────────────────────────────────┘
                                          → back to FREE
```

---

## Part B — FetchPage (Cache Hit & Cache Miss)

```mermaid
sequenceDiagram
    autonumber
    actor Caller as TableIterator / InsertOperation
    participant BPM as BufferPoolManager
    participant PageTable as _pageTable (Dict<pageId, frameId>)
    participant FreeList as _freeList (Queue<frameId>)
    participant Replacer as IReplacer (LRU/Clock)
    participant Disk as IDiskManager
    participant Frame as BufferFrame

    Caller->>BPM: Load page into memory [ FetchPage(pageId: 5) ]

    alt pageId < 0
        BPM-->>Caller: Throw ArgumentOutOfRangeException
    end

    BPM->>PageTable: Check if page already loaded [ pageTable.ContainsKey(5) ]

    alt CACHE HIT
        PageTable-->>BPM: Return frameId = 2
        BPM->>Frame: Increment pin count [ frame.PinCount++ ]
        BPM->>Replacer: Prevent eviction [ Pin(frameId: 2) ]
        BPM-->>Caller: Return page (PinCount now ≥ 2)

    else CACHE MISS
        PageTable-->>BPM: Not found

        alt Free frame available in _freeList
            BPM->>FreeList: Get free frame [ freeList.Dequeue() ]
            FreeList-->>BPM: Return frameId = 7 (pristine, no eviction needed)

        else No free frame — must evict
            BPM->>Replacer: Request eviction candidate [ Victim(out victimFrameId) ]

            alt No evictable frames (all pinned)
                Replacer-->>BPM: returns false
                BPM-->>Caller: Throw BufferPoolFullException
            else Victim found (e.g., frameId = 3, pageId = 99)
                Replacer-->>BPM: returns true, victimFrameId = 3

                opt Frame is dirty (IsDirty == true)
                    BPM->>Disk: Write dirty page to disk first [ WritePage(fileId, 99, data) ]
                    Disk-->>BPM: Write complete
                end

                BPM->>PageTable: Remove old mapping [ pageTable.Remove(99) ]
                BPM->>Frame: Reset frame [ frame.Reset() ]
            end
        end

        Note over BPM: frameId chosen: 7 (or evicted frame 3)
        BPM->>Disk: Read page from disk [ ReadPage(fileId, 5, buffer) ]
        Disk-->>BPM: buffer filled with 4096 bytes

        BPM->>Frame: Load data [ frame.PageId=5, PinCount=1, IsDirty=false ]
        BPM->>PageTable: Record mapping [ pageTable[5] = frameId ]
        BPM->>Replacer: Protect new frame [ Pin(frameId) ]
        BPM-->>Caller: Return page (PinCount = 1)
    end
```

---

## Part C — UnpinPage

```mermaid
sequenceDiagram
    autonumber
    actor Caller as TableIterator / InsertOperation
    participant BPM as BufferPoolManager
    participant PageTable as _pageTable
    participant Frame as BufferFrame
    participant Replacer as IReplacer

    Caller->>BPM: Release page [ UnpinPage(pageId: 5, isDirty: true) ]

    alt pageId not in pageTable
        BPM-->>Caller: Throw KeyNotFoundException
    end

    BPM->>PageTable: Lookup frameId for pageId=5
    PageTable-->>BPM: frameId = 7

    BPM->>Frame: Check pin count
    alt PinCount already 0
        BPM-->>Caller: Throw InvalidOperationException ("already unpinned")
    end

    BPM->>Frame: Decrement pin count [ frame.PinCount-- ]

    opt isDirty == true
        BPM->>Frame: Set dirty flag [ frame.IsDirty = true ]
        Note over Frame: Dirty flag is "sticky" — once set to true,<br/>a subsequent UnpinPage(..., false) does NOT clear it.
    end

    opt PinCount drops to 0
        BPM->>Replacer: Allow eviction [ Replacer.Unpin(frameId: 7) ]
        Note over Replacer: Frame 7 is now evictable
    end

    BPM-->>Caller: OK (page released)
```

---

## Part D — FlushPage & FlushAllPages

```mermaid
sequenceDiagram
    autonumber
    actor Caller as DirtyPageWriter / Transaction Manager
    participant BPM as BufferPoolManager
    participant Frame as BufferFrame
    participant Disk as IDiskManager

    %% FlushPage (single page)
    Caller->>BPM: Force write single page [ FlushPage(pageId: 5) ]

    alt pageId not in pool
        BPM-->>Caller: Throw KeyNotFoundException
    else page is CLEAN (IsDirty == false)
        Note over BPM: No-op — page already consistent with disk
        BPM-->>Caller: OK (WritePage NOT called)
    else page is DIRTY
        BPM->>Disk: Write page data to disk [ WritePage(fileId, 5, frame.data) ]
        Disk-->>BPM: Write complete
        BPM->>Frame: Clear dirty flag [ frame.IsDirty = false ]
        BPM-->>Caller: OK
    end

    %% FlushAllPages
    Caller->>BPM: Flush entire pool [ FlushAllPages() ]
    Note over BPM: Iterate all frames in _frames[]
    loop For each frame in _frames
        alt frame.IsDirty == true
            BPM->>Disk: WritePage(fileId, frame.PageId, frame.data)
            Disk-->>BPM: Write complete
            BPM->>Frame: frame.IsDirty = false
        else frame is clean or free
            Note over BPM: Skip
        end
    end
    BPM-->>Caller: All dirty pages flushed
```

---

## Part E — NewPage & DeletePage

```mermaid
sequenceDiagram
    autonumber
    actor Caller as InsertOperation / Catalog Manager
    participant BPM as BufferPoolManager
    participant FreeList as _freeList
    participant Replacer as IReplacer
    participant PageTable as _pageTable
    participant Frame as BufferFrame
    participant Disk as IDiskManager

    %% NewPage
    Caller->>BPM: Create a new page [ NewPage() ]
    Note over BPM: Allocate a new pageId (monotonically increasing)

    alt Free frame OR evictable frame available
        BPM->>FreeList: Try freeList.Dequeue() or Replacer.Victim()
        Note over BPM: Same eviction logic as FetchPage cache miss
        BPM->>Frame: frame.PageId = newId, PinCount = 1, IsDirty = false
        BPM->>PageTable: pageTable[newId] = frameId
        BPM->>Replacer: Pin(frameId)
        BPM-->>Caller: Return newPageId (pinned, zero-filled)
    else No frames available (all pinned)
        BPM-->>Caller: Throw BufferPoolFullException
    end

    %% DeletePage
    Caller->>BPM: Remove page from pool [ DeletePage(pageId: 5) ]

    alt pageId not in pool
        BPM-->>Caller: Return false (or throw — implementation decision)
    else page is pinned (PinCount > 0)
        BPM-->>Caller: Throw InvalidOperationException ("cannot delete pinned page")
    else page is unpinned
        BPM->>PageTable: Remove mapping [ pageTable.Remove(5) ]
        BPM->>Replacer: Pin(frameId) — remove from evictable list
        BPM->>Frame: Reset frame [ frame.Reset() — PageId=-1, PinCount=0, IsDirty=false ]
        BPM->>FreeList: Return frame to free list [ freeList.Enqueue(frameId) ]
        BPM-->>Caller: Return true (frame recycled)
    end
```

---

# Mapping to Test Cases

## 9A. FetchPage

| Test | Diagram step |
|:-----|:------------|
| `FetchPage_CacheHit_ReturnsSamePageObject` | Part B: Cache Hit path |
| `FetchPage_CacheHit_PinCountIncrements` | Part B step 5 (`PinCount++`) |
| `FetchPage_CacheHit_ReadPageCalledOnlyOnce` | Part B: Cache Hit — disk not called again |
| `FetchPage_CacheMiss_FreeFrame_CallsReadPage` | Part B: Cache Miss → ReadPage |
| `FetchPage_CacheMiss_Eviction_DirtyFrame_WritesFirst` | Part B: eviction → WritePage before ReadPage |
| `FetchPage_CacheMiss_Eviction_OldPageRemovedFromTable` | Part B: `pageTable.Remove(victimPageId)` |
| `FetchPage_NoFrameAvailable_ThrowsBufferPoolFullException` | Part B: Replacer returns false |
| `FetchPage_NegativePageId_ThrowsArgumentOutOfRangeException` | Part B step 3 guard |

## 9B. UnpinPage

| Test | Diagram step |
|:-----|:------------|
| `UnpinPage_IsDirtyTrue_MarksFrameDirty` | Part C: `isDirty=true` sets dirty flag |
| `UnpinPage_CleanUnpinAfterDirtyUnpin_StillDirty` | Part C: "sticky" dirty note |
| `UnpinPage_PinCountZero_FrameAddedToReplacer` | Part C: `Replacer.Unpin()` when PinCount=0 |
| `UnpinPage_PageNotInPool_ThrowsKeyNotFoundException` | Part C step 4 |
| `UnpinPage_PinCountAlreadyZero_ThrowsInvalidOperationException` | Part C step 9 |

## 9C/D. Flush

| Test | Diagram step |
|:-----|:------------|
| `FlushPage_DirtyPage_CallsWritePage` | Part D: dirty path |
| `FlushPage_CleanPage_DoesNotCallWritePage` | Part D: clean path (no-op) |
| `FlushAllPages_MixedDirty_WritesOnlyDirty` | Part D loop: conditional write |
| `FlushAllPages_ClearsAllDirtyFlags` | Part D: `IsDirty = false` after flush |

## 9E/F. NewPage & DeletePage

| Test | Diagram step |
|:-----|:------------|
| `NewPage_ReturnsNonNegativePageId` | Part E: new monotonic ID |
| `NewPage_NewPageIsPinned` | Part E: `PinCount = 1` |
| `DeletePage_PinnedPage_ThrowsInvalidOperationException` | Part E Delete: pinned check |
| `DeletePage_UnpinnedPage_FrameReturnedToFreeList` | Part E Delete: frame recycled |
| `DeletePage_AfterDelete_FetchLoadsFromDiskAgain` | Part E Delete → Part B Cache Miss |
# Replacer Algorithms: LRUReplacer & ClockReplacer

Context: The `IReplacer` interface is the **eviction policy engine** inside the `BufferPoolManager`. When the buffer pool has no free frames and a new page must be loaded, the replacer decides which existing frame to evict. This diagram details the internal state transitions of both `LRUReplacer` and `ClockReplacer`. This is the exact flow exercised by `LRUReplacerTests` and `ClockReplacerTests`.

---

## Part A — LRUReplacer (Least Recently Used)

### Internal Structure
```
_lruList (LinkedList<int>): [3] ↔ [1] ↔ [2]    ← head = LRU (oldest), tail = MRU (newest)
_lruMap  (Dictionary<int, Node>): { 3→Node, 1→Node, 2→Node }
```
Frames in this list are **evictable** (unpinned). Pinned frames are NOT in the list.

```mermaid
sequenceDiagram
    autonumber
    actor BPM as BufferPoolManager
    participant LRU as LRUReplacer

    Note over BPM,LRU: Initial state: capacity=5, all frames pinned (list empty)

    %% ─── Unpin (makes evictable) ───
    BPM->>LRU: Frame 1 released by caller [ Unpin(frameId: 1) ]
    Note over LRU: Add frame 1 to TAIL of list (most recently used)<br/>List: [1]  Size=1

    BPM->>LRU: Frame 2 released [ Unpin(frameId: 2) ]
    Note over LRU: Add frame 2 to TAIL<br/>List: [1] ↔ [2]  Size=2

    BPM->>LRU: Frame 3 released [ Unpin(frameId: 3) ]
    Note over LRU: Add frame 3 to TAIL<br/>List: [1] ↔ [2] ↔ [3]  Size=3

    %% ─── Re-access (LRU order update) ───
    BPM->>LRU: Frame 1 accessed again [ Unpin(frameId: 1) (idempotent re-unpin) ]
    Note over LRU: Move frame 1 from HEAD to TAIL (now MRU)<br/>List: [2] ↔ [3] ↔ [1]  Size=3

    %% ─── Victim (eviction) ───
    BPM->>LRU: Request a frame to evict [ Victim(out frameId) ]
    Note over LRU: Pick HEAD = frame 2 (LRU = least recently used)<br/>Remove from list and map
    LRU-->>BPM: returns true, frameId = 2<br/>List: [3] ↔ [1]  Size=2

    BPM->>LRU: Victim again [ Victim(out frameId) ]
    LRU-->>BPM: returns true, frameId = 3<br/>List: [1]  Size=1

    %% ─── Pin (makes non-evictable) ───
    BPM->>LRU: Frame 1 now in use [ Pin(frameId: 1) ]
    Note over LRU: Remove frame 1 from list (if present)<br/>List: []  Size=0

    %% ─── Victim when empty ───
    BPM->>LRU: Victim on empty list [ Victim(out frameId) ]
    LRU-->>BPM: returns false (no evictable frames)

    %% ─── Invalid input ───
    BPM->>LRU: Pin with invalid id [ Pin(frameId: -1) ]
    LRU-->>BPM: Throw ArgumentOutOfRangeException
```

---

## Part B — ClockReplacer (Clock / Second Chance)

### Internal Structure
```
_frames array (size = capacity):
  Index:    [0]     [1]     [2]     [3]
  State:    {pinned=false, ref=1}  {pinned=false, ref=0}  {pinned=true}  {pinned=false, ref=1}
_clockHand: integer index that advances around the ring
```

```mermaid
sequenceDiagram
    autonumber
    actor BPM as BufferPoolManager
    participant Clock as ClockReplacer

    Note over BPM,Clock: Initial state: capacity=3<br/>All frames: pinned=true, ref=0<br/>_clockHand = 0

    %% ─── Unpin (set ref bit = 1) ───
    BPM->>Clock: Frame 0 released [ Unpin(frameId: 0) ]
    Note over Clock: Frame 0: pinned=false, ref=1

    BPM->>Clock: Frame 1 released [ Unpin(frameId: 1) ]
    Note over Clock: Frame 1: pinned=false, ref=1

    %% ─── Victim: sweep finds ref=1 first, clears it ───
    BPM->>Clock: Evict a frame [ Victim(out frameId) ]
    Note over Clock: Clock sweep from hand=0:<br/>→ Frame 0: ref=1 → clear to ref=0, advance hand to 1<br/>→ Frame 1: ref=1 → clear to ref=0, advance hand to 2<br/>→ Frame 2: pinned=true → skip, advance hand to 0<br/>→ Frame 0: ref=0 → EVICT IT, advance hand to 1
    Clock-->>BPM: returns true, frameId = 0

    %% ─── Victim: ref=0 evicted immediately ───
    BPM->>Clock: Evict again [ Victim(out frameId) ]
    Note over Clock: Clock hand=1:<br/>→ Frame 1: ref=0 → EVICT immediately
    Clock-->>BPM: returns true, frameId = 1

    %% ─── Pin (exclude from eviction) ───
    BPM->>Clock: Frame 2 in use (keep) [ Pin(frameId: 2) ]
    Note over Clock: Frame 2: pinned=true (already was)

    BPM->>Clock: Victim with all pinned [ Victim(out frameId) ]
    Note over Clock: Full sweep — all pinned → no candidate found
    Clock-->>BPM: returns false
```

---

# Key Differences: LRU vs Clock

| Property | LRUReplacer | ClockReplacer |
|:---------|:-----------|:-------------|
| **Eviction order** | Strictly LRU (oldest unpinned) | Approximate LRU (second-chance) |
| **Data structure** | `LinkedList` + `Dictionary` | Circular `bool[]` array |
| **Memory overhead** | O(n) per frame (pointer nodes) | O(1) per frame (single bit) |
| **Unpin semantics** | Moves to tail of LRU list | Sets `ref_bit = 1` |
| **Victim scan** | O(1) — always head of list | O(n) worst case (full sweep) |
| **Re-access grace** | Re-unpin moves frame to MRU tail | Sets `ref_bit = 1` (gets one more chance) |

---

# Mapping to Test Cases

## LRUReplacer

| Test | Diagram step |
|:-----|:------------|
| `Victim_NoUnpinnedFrames_ReturnsFalse` | Step 17 (empty list) |
| `Victim_OneUnpinnedFrame_ReturnsTrueAndId` | Steps 3–8 (single frame) |
| `Victim_MultipleUnpinned_ReturnsLRU` | Steps 3–10 (multi-frame, picks head) |
| `Victim_AfterReAccess_SkipsRecentlyUsed` | Steps 11–14 (re-unpin moves to tail) |
| `Victim_SequentialCalls_CorrectOrder` | Steps 11–14 × 3 |
| `Pin_UnpinnedFrame_RemovesFromEvictableList` | Steps 15–16 |
| `Unpin_AlreadyUnpinned_IsIdempotent` | Step 11 — re-unpin idempotent (moves to tail) |
| `Pin_InvalidFrameId_ThrowsArgumentOutOfRangeException` | Step 19 |
| `Size_AfterUnpin_Increases` | Steps 3–8 → `Size == 1` |
| `InterleavePattern_LRUMaintained` | Steps 11–14 interleave |

## ClockReplacer

| Test | Diagram step |
|:-----|:------------|
| `Victim_NoFrames_ReturnsFalse` | Step 19 (all pinned) |
| `Victim_OneFrameRefBitOne_ClearsAndReturnsItself` | Steps 7–14 (full sweep, clears, then evicts) |
| `Victim_F0RefBit1_F1RefBit0_ReturnsF1` | Steps 7–13 (F0 cleared, F1 evicted next cycle) |
| `Victim_AllRefBitOne_FullSweepThenEvicts` | Steps 7–14 (clears all, then evicts frame 0) |
| `ClockHand_WrapAround_CorrectCycle` | Steps 7–19 (wrap-around) |
| `Pin_Frame_ExcludesFromEviction` | Step 16 — pinned frame skipped in sweep |
| `Unpin_InvalidFrameId_ThrowsArgumentOutOfRangeException` | (guard on frameId bounds) |
