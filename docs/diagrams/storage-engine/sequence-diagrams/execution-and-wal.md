# TableIterator — Sequential Scan (Access Method)

Context: `TableIterator` implements `IRowIterator` and performs a **full table scan** by iterating through all pages of a `TableHeap` in order. The [page-fetching.md](overview/page-fetching.md) overview shows the BPM from the perspective of a single page fetch; this diagram shows the **complete scan lifecycle**: `Open()`, `HasNext()`, `Next()` across multiple pages with deleted slot skipping, and `Close()` with resource cleanup. This is the exact flow exercised by `TableIteratorTests`.

---

## Part A — Complete Scan Lifecycle

```mermaid
sequenceDiagram
    autonumber
    actor Query as Query Executor
    participant Iter as TableIterator
    participant BPM as IBufferPoolManager (mock)
    participant Page as SlottedPage (in BPM)

    Note over Iter: Initial state: _isOpen=false, _currentPageId=-1, _currentSlotId=-1

    %% ─── Open ───
    Query->>Iter: Begin scan [ Open() ]

    alt already open
        Iter-->>Query: Throw InvalidOperationException ("already open")
    end

    Iter->>Iter: _isOpen = true<br/>_currentPageId = firstPageId (e.g., 0)<br/>_currentSlotId = -1

    Iter-->>Query: OK (positioned before first record)

    %% ─── HasNext (advance scan cursor) ───
    Query->>Iter: Is there a next record? [ HasNext() ]

    alt _isOpen == false
        Iter-->>Query: Throw InvalidOperationException ("not open")
    end

    Note over Iter: Advance _currentSlotId to find next valid slot

    loop Scan until valid slot or no more pages
        Iter->>Iter: _currentSlotId++

        alt _currentSlotId < page.SlotCount
            Iter->>Page: Check if slot is valid [ page.GetSlotLength(_currentSlotId) ]
            alt length == 0 (deleted slot)
                Note over Iter: Skip — continue to next slotId
            else length > 0 (valid slot)
                Note over Iter: Found a valid record → HasNext returns true
            end
        else _currentSlotId >= SlotCount (end of current page)
            Iter->>BPM: Release current page [ UnpinPage(_currentPageId, isDirty: false) ]
            BPM-->>Iter: OK

            Iter->>Iter: Advance to next page: _currentPageId++

            alt no more pages (pageId >= totalPages or sentinel)
                Note over Iter: No more records → HasNext returns false
            else load next page
                Iter->>BPM: Load next page [ FetchPage(_currentPageId) ]
                BPM-->>Iter: Return Page (pinned)
                Iter->>Iter: _currentSlotId = -1 (reset for new page)
                Note over Iter: Continue scanning from slot 0 of new page
            end
        end
    end

    Iter-->>Query: true (record found) OR false (exhausted)

    %% ─── Next (return record) ───
    Query->>Iter: Get the current record [ Next() ]

    alt _isOpen == false
        Iter-->>Query: Throw InvalidOperationException ("not open")
    else HasNext() == false
        Iter-->>Query: Throw InvalidOperationException ("no more records")
    end

    Iter->>Page: Read slot data [ page.GetSlotOffset(), page.GetSlotLength() ]
    Page-->>Iter: offset, length

    Iter->>Page: Extract bytes from page data area
    Page-->>Iter: byte[] recordData

    Iter->>Iter: Construct Tuple(recordData)<br/>tuple.Rid = new RID(_currentPageId, _currentSlotId)

    Iter-->>Query: Return Tuple (with correct RID)

    %% ─── Idempotent HasNext ───
    Note over Query: HasNext() can be called multiple times without advancing
    Query->>Iter: HasNext() called 3 times without Next()
    Iter-->>Query: true / true / true (cursor not moved)

    %% ─── Close ───
    Query->>Iter: End scan [ Close() ]

    alt already closed or never opened
        Note over Iter: Idempotent — no exception on double close
    else pages still pinned
        Iter->>BPM: Release all pinned pages [ UnpinPage(pageId, false) for each pinned page ]
        BPM-->>Iter: OK
    end

    Iter->>Iter: _isOpen = false, _currentPageId = -1
    Iter-->>Query: OK
```

---

## Part B — Multi-Page Scan with Deleted Slot Skipping

```mermaid
sequenceDiagram
    autonumber
    actor Query as Query Executor
    participant Iter as TableIterator
    participant BPM as IBufferPoolManager
    participant P0 as Page 0 (SlotCount=3)
    participant P1 as Page 1 (SlotCount=2)

    Note over P0: slot 0: DELETED (length=0)<br/>slot 1: valid  (length=50)<br/>slot 2: valid  (length=30)

    Note over P1: slot 0: valid  (length=80)<br/>slot 1: DELETED (length=0)

    Iter->>BPM: FetchPage(0)
    BPM-->>Iter: P0 (pinned)

    %% First HasNext → skips deleted slot 0
    Iter->>P0: GetSlotLength(0) == 0 → skip
    Iter->>P0: GetSlotLength(1) == 50 → valid
    Note over Iter: HasNext → true, cursor at (page=0, slot=1)

    %% Next returns slot 1
    Iter-->>Query: Tuple{ data=slot1_data, Rid=RID(0,1) }

    %% Second HasNext → slot 2
    Iter->>P0: GetSlotLength(2) == 30 → valid
    Note over Iter: HasNext → true, cursor at (page=0, slot=2)
    Iter-->>Query: Tuple{ data=slot2_data, Rid=RID(0,2) }

    %% Third HasNext → end of page 0, move to page 1
    Note over Iter: slot=3 >= SlotCount=3 → end of page 0
    Iter->>BPM: UnpinPage(0, false)
    Iter->>BPM: FetchPage(1)
    BPM-->>Iter: P1 (pinned)

    %% slot 0 of page 1 is valid
    Iter->>P1: GetSlotLength(0) == 80 → valid
    Iter-->>Query: Tuple{ data=slot0_p1_data, Rid=RID(1,0) }

    %% slot 1 of page 1 is deleted → end of data
    Iter->>P1: GetSlotLength(1) == 0 → skip
    Note over Iter: No more slots, no more pages → HasNext = false
    Iter->>BPM: UnpinPage(1, false)
    Iter-->>Query: HasNext → false

    Note over Query: Total records yielded: 3 (2 from page 0, 1 from page 1)<br/>Deleted slots were skipped transparently.
```

---

## Part C — Reopen After Close

```mermaid
sequenceDiagram
    autonumber
    actor Query as Query Executor
    participant Iter as TableIterator

    Query->>Iter: Open() → scan all → Close()
    Note over Iter: After Close: _isOpen=false, _currentPageId=-1

    Query->>Iter: Open() again [ second scan ]
    Note over Iter: Resets all state:<br/>_currentPageId = firstPageId<br/>_currentSlotId = -1<br/>_isOpen = true

    Note over Query: Second scan returns ALL records from the beginning.<br/>This is critical for nested loop joins where the inner table is scanned multiple times.
```

---

# Design Invariants

> [!IMPORTANT]
> **All fetched pages MUST be unpinned.** If `Close()` is called while pages are pinned, the iterator must unpin them all. Failure to do so causes `BufferPoolFullException` in long-running scans.

> [!NOTE]
> **`HasNext()` is idempotent.** Calling it multiple times without `Next()` must not advance the cursor. This is required for the standard `while(iter.HasNext()) { tuple = iter.Next(); ... }` pattern.

> [!NOTE]
> **RID assignment in Next().** The `Tuple.Rid` is set to `new RID(currentPageId, currentSlotId)` at read time — this matches the physical location and is used by index scans to correlate with the B+ tree leaf page lookup.

---

# Mapping to Test Cases

| Test | Diagram step |
|:-----|:------------|
| `Open_AlreadyOpen_ThrowsInvalidOperationException` | Part A step 4 |
| `HasNext_EmptyTable_ReturnsFalse` | Part A: all pages empty → false |
| `HasNext_TableWithRecords_ReturnsTrue` | Part A steps 10–16 |
| `HasNext_CalledMultipleTimes_SameResult` | Part A: idempotent note |
| `HasNext_BeforeOpen_ThrowsInvalidOperationException` | Part A step 8 |
| `Next_SingleRecord_ReturnsCorrectTuple` | Part A steps 17–24 |
| `Next_SkipsDeletedSlots` | Part B: slot 0 deleted → skip |
| `Next_SpansMultiplePages_AllRecordsReturned` | Part B: full multi-page scan |
| `Next_ReturnedTuple_HasCorrectRID` | Part A step 22: `Rid = RID(page, slot)` |
| `Next_WhenHasNextFalse_ThrowsInvalidOperationException` | Part A step 19 |
| `Close_ReleasesAllFetchedFrames` | Part A step 28 + Part B UnpinPage calls |
| `Close_BeforeOpen_NoException` | Part A: idempotent close |
| `Close_AfterClose_IsIdempotent` | Part A: idempotent close |
| `Reopen_AfterClose_RestartsFromBeginning` | Part C |
# FreeSpaceMap & WalBuffer & WalWriter

Context: Three advanced components that support the insert workflow. The [insert-record.md](overview/insert-record.md) overview calls `FSM.LocateFreePage()` and `WAL.AppendLog()` as single steps. This diagram zooms into the **internal mechanics** of each: how `FreeSpaceMap` tracks per-page free bytes, how `WalBuffer` accumulates log records in memory, and how `WalWriter` orchestrates LSN assignment and the WAL-before-data rule. This covers `FreeSpaceMapTests`, `WalBufferTests`, and `WalWriterTests`.

---

## Part A — FreeSpaceMap

```mermaid
sequenceDiagram
    autonumber
    actor Exec as InsertOperation
    participant FSM as FreeSpaceMap

    Note over FSM: Internal state: _fsmData[pageId] = freeBytes<br/>Example: 5 pages → [3800, 0, 2000, 4080, 100]

    %% ─── LocateFreePage ───
    Exec->>FSM: Find page with enough space [ LocateFreePage(requiredSpace: 500) ]

    alt requiredSpace <= 0
        FSM-->>Exec: Throw ArgumentException ("requiredSpace must be positive")
    end

    Note over FSM: Linear scan: check each page entry<br/>  page 0: 3800 ≥ 500 ✅ → return immediately

    FSM-->>Exec: Return pageId = 0

    %% ─── All pages full ───
    Exec->>FSM: LocateFreePage(requiredSpace: 5000)
    Note over FSM: Scan: 3800 < 5000, 0 < 5000, 2000 < 5000, 4080 < 5000, 100 < 5000<br/>No match found
    FSM-->>Exec: Return -1 (no suitable page)

    %% ─── UpdateFsmSpace ───
    Exec->>FSM: Update after insert [ UpdateFsmSpace(pageId: 0, freeSpace: 3300) ]

    alt pageId < 0 or pageId >= totalPages
        FSM-->>Exec: Throw ArgumentOutOfRangeException
    else freeSpace < 0
        FSM-->>Exec: Throw ArgumentException
    end

    FSM->>FSM: _fsmData[0] = 3300
    FSM-->>Exec: OK

    %% ─── MarkPageFull ───
    Exec->>FSM: Mark page as full [ MarkPageFull(pageId: 2) ]
    alt pageId out of range
        FSM-->>Exec: Throw ArgumentOutOfRangeException
    end
    FSM->>FSM: _fsmData[2] = 0
    FSM-->>Exec: OK
    Note over FSM: LocateFreePage will now skip page 2 until UpdateFsmSpace restores it

    %% ─── AddNewPages (after ExtentManager allocates new pages) ───
    Exec->>FSM: Register new pages [ AddNewPages(count: 8) ]
    alt count < 0
        FSM-->>Exec: Throw ArgumentException
    end
    FSM->>FSM: Append 8 new entries to _fsmData[] (each = PAGE_SIZE - HEADER_SIZE = 4080)
    FSM-->>Exec: OK — totalPages increased by 8
```

---

## Part B — WalBuffer

```mermaid
sequenceDiagram
    autonumber
    actor Writer as WalWriter
    participant Buf as WalBuffer

    Note over Buf: Internal state:<br/>_buffer: byte[] (capacityBytes)<br/>_writePos: int (current fill position)<br/>_currentLsn: long

    %% ─── Write ───
    Writer->>Buf: Append log record [ Write(record: LogRecord) ]

    alt record is null
        Buf-->>Writer: Throw ArgumentNullException
    else buffer is full
        Buf-->>Writer: Throw BufferOverflowException
    end

    Note over Buf: Serialize LogRecord into _buffer at _writePos<br/>_writePos advances by record size<br/>_currentLsn = record.Lsn

    Buf-->>Writer: OK

    %% ─── IsFull / GetCurrentLsn ───
    Writer->>Buf: Check capacity [ IsFull ]
    Buf-->>Writer: (_writePos >= capacityBytes)

    Writer->>Buf: Get latest LSN [ GetCurrentLsn() ]
    Buf-->>Writer: _currentLsn (0 if no writes yet)

    %% ─── Read (non-destructive) ───
    Writer->>Buf: Read all buffered records [ Read() ]
    Buf-->>Writer: Return IReadOnlyList<LogRecord> (does NOT clear buffer)
    Note over Buf: Calling Read() twice returns same records

    %% ─── Flush (destructive — called by WalWriter when flushing to disk) ───
    Writer->>Buf: Clear after disk write [ Flush() ]
    Note over Buf: Reset: _writePos = 0, IsFull = false<br/>BUT: _currentLsn is PRESERVED (LSN is monotonic, not reset)
    Buf-->>Writer: OK
    Note over Writer: After Flush, can Write again (buffer is empty)
```

---

## Part C — WalWriter (LSN Generation & WAL Rule Enforcement)

```mermaid
sequenceDiagram
    autonumber
    actor BPM as BufferPoolManager / InsertOperation
    participant WAL as WalWriter
    participant Buf as WalBuffer (IWalBuffer)
    participant Disk as IDiskManager

    Note over WAL: Internal: _flushedLsn (highest LSN written to disk)

    %% ─── AppendLog ───
    BPM->>WAL: Write a log entry [ AppendLog(record: LogRecord) ]

    alt record is null
        WAL-->>BPM: Throw ArgumentNullException
    end

    WAL->>WAL: Assign monotonically increasing LSN [ _lsnCounter++ ]
    Note over WAL: record.Lsn = _lsnCounter<br/>LSN sequence: 1, 2, 3, 4, ... (never reset)

    WAL->>Buf: Append to in-memory buffer [ walBuffer.Write(record) ]

    alt walBuffer.IsFull (auto-flush trigger)
        Note over WAL: Buffer full → must flush before new write
        WAL->>Disk: Write buffer contents to log file [ WritePage(logFileId, ...) ]
        Disk-->>WAL: Write confirmed
        WAL->>Buf: Clear buffer [ walBuffer.Flush() ]
        WAL->>WAL: Update _flushedLsn = current LSN
        WAL->>Buf: Now write the new record [ walBuffer.Write(record) ]
    end

    Buf-->>WAL: Write confirmed
    WAL-->>BPM: Return LSN (e.g., 7)

    %% ─── FlushBuffer (explicit flush) ───
    BPM->>WAL: Force log to disk [ FlushBuffer() ]

    alt walBuffer is empty
        Note over WAL: No-op — nothing to write
        WAL-->>BPM: OK (WritePage NOT called)
    else has data
        WAL->>Disk: Write buffered records to log file [ WritePage(logFileId, ...) ]
        Disk-->>WAL: Write confirmed
        WAL->>Buf: Clear buffer [ walBuffer.Flush() ]
        WAL->>WAL: _flushedLsn = walBuffer.GetCurrentLsn()
        WAL-->>BPM: OK (2nd flush immediately after: no-op)
    end

    %% ─── EnforceWalRule ───
    Note over BPM: Called before BPM writes a dirty page to disk.<br/>Rule: log must be flushed up to pageLsn BEFORE page data.

    BPM->>WAL: Ensure log durability [ EnforceWalRule(pageLsn: 10) ]

    alt pageLsn > _currentLsn (impossible — page hasn't been logged yet)
        WAL-->>BPM: Throw InvalidOperationException
    else pageLsn <= _flushedLsn (log already on disk)
        Note over WAL: No action needed — WAL constraint already satisfied
        WAL-->>BPM: OK (FlushBuffer NOT called)
    else pageLsn > _flushedLsn (unflushed log records exist up to pageLsn)
        WAL->>WAL: FlushBuffer() — write log to disk
        WAL-->>BPM: OK (now safe to write dirty page)
    end
```

---

# Key Concepts

## FreeSpaceMap Accuracy

> [!NOTE]
> `FreeSpaceMap` stores an **approximation** of free space — it is updated after each insert via `UpdateFsmSpace()`, but does not perfectly account for fragmentation before `DefragmentPage()` is called. The actual free space is always confirmed by `SlottedPage.GetFreeSpace()` before allocation.

## WAL LSN Invariants

1. **Monotonically increasing:** Each `AppendLog()` call gets a higher LSN than all previous calls. LSN is **never reset**, even after `Flush()`.
2. **WAL-before-data:** `EnforceWalRule(pageLsn)` ensures `_flushedLsn >= pageLsn` before any dirty page is written to disk.
3. **Auto-flush:** When `WalBuffer.IsFull`, the `WalWriter` automatically flushes before accepting the next record — ensuring LSN continuity.

---

# Mapping to Test Cases

## FreeSpaceMap

| Test | Step |
|:-----|:-----|
| `LocateFreePage_SingleFreePageSufficient_ReturnsPageId` | Part A steps 3–9 |
| `LocateFreePage_AllPagesFull_ReturnsNegativeOne` | Part A steps 10–13 |
| `LocateFreePage_NeededZero_ThrowsArgumentException` | Part A step 4 |
| `UpdateFsmSpace_UpdatesCorrectPage` | Part A steps 14–20 |
| `UpdateFsmSpace_NegativeFreeBytes_Throws` | Part A step 17 |
| `MarkPageFull_PageNotReturnedByLocate` | Part A steps 21–25 |
| `AddNewPages_IncreasesCapacity` | Part A steps 26–30 |

## WalBuffer

| Test | Step |
|:-----|:-----|
| `Write_MultipleRecords_OrderPreserved` | Part B steps 3–11 |
| `Write_WhenFull_ThrowsBufferOverflowException` | Part B step 6 |
| `GetCurrentLsn_Initially_ReturnsZero` | Part B step 13 (0 if no writes) |
| `Read_DoesNotClearBuffer` | Part B steps 14–16 |
| `Flush_ClearsAllRecords` | Part B steps 17–20 |
| `GetCurrentLsn_AfterFlush_LsnPreserved` | Part B step 20 note |

## WalWriter

| Test | Step |
|:-----|:-----|
| `AppendLog_LsnIsMonotonicallyIncreasing` | Part C step 7 |
| `AppendLog_WritesToBuffer` | Part C step 9 |
| `AppendLog_BufferFull_AutoFlushesAndContinues` | Part C steps 11–18 |
| `AppendLog_LsnContinuesAfterAutoFlush` | Part C step 7 + note |
| `FlushBuffer_EmptyBuffer_NoDiskWrite` | Part C step 22 |
| `FlushBuffer_ClearsBuffer_SecondFlushNoWrite` | Part C steps 24–28 |
| `EnforceWalRule_PageLsnBelowCurrentFlushedLsn_NoFlush` | Part C step 33 |
| `EnforceWalRule_PageLsnEqualUnflushedLsn_FlushesBuffer` | Part C step 36 |
| `EnforceWalRule_PageLsnAboveCurrentLsn_ThrowsException` | Part C step 32 |
