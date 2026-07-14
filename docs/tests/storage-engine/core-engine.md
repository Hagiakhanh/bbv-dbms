# 🧪 Test Cases Design — TDD Full Specification (Layer 2: Core Engine)
## Storage Engine DBMS · C# (.NET) · xUnit · Moq

> **Priority:** P1 (I/O) → P2 (Buffer) → P3 (Storage)

---

## Legend

| Term | Meaning |
|---------|---------|
| `Input` | Input / precondition / system state |
| `Output` | Expected output / return value |
| `Exception` | Expected exception type |
| `Side Effect` | Side effect (mock verify, state change) |
| `Happy Path` | Normal execution without errors |
| `Error Case` | Error / exception case |
| `Boundary Case`| Boundary / edge case |

---

## ═══════════════════════════════════════
## ⚙️ LAYER 2 — CORE ENGINE
## ═══════════════════════════════════════

---

### 📁 4. DataFileRegistry

```csharp
// Test class: DataFileRegistryTests
// No external I/O — pure in-memory registry
// Fixture: new DataFileRegistry() fresh per test
```

| # | Test Name | Input | Output / Exception / Side Effect | Type |
|---|-----------|-------|----------------------------------|------|
| 1 | `RegisterFile_ValidPath_ReturnsNonNegativeId` | `registry.RegisterFile("a.db", FileType.Data)` | `fileId >= 0` | Happy Path |
| 2 | `RegisterFile_TwoFiles_IdsAreDistinct` | Register `"a.db"` → Register `"b.db"` | `id1 != id2` | Happy Path |
| 3 | `RegisterFile_IdsAreMonotonicallyIncreasing` | Register 5 files | `ids[0] < ids[1] < ... < ids[4]` | Happy Path |
| 4 | `RegisterFile_NullPath_ThrowsArgumentNullException` | `RegisterFile(null, FileType.Data)` | `Exception: ArgumentNullException` param: "path" | Error Case |
| 5 | `RegisterFile_EmptyPath_ThrowsArgumentException` | `RegisterFile("", FileType.Data)` | `Exception: ArgumentException` | Error Case |
| 6 | `RegisterFile_SamePathTwice_ThrowsInvalidOperationException` | Register `"a.db"`, then Register `"a.db"` again | `Exception: InvalidOperationException` on 2nd call | Error Case |
| 7 | `RegisterFile_DifferentFileTypes_BothSucceed` | Register `"a.db"` with `Data`, Register `"b.db"` with `Log` | 2 distinct IDs, no exception | Happy Path |
| 8 | `RegisterFile_WhitespaceOnlyPath_ThrowsArgumentException` | `RegisterFile("   ", FileType.Data)` | `Exception: ArgumentException` | Error Case |
| 9 | `IsRegistered_AfterRegister_ReturnsTrue` | Register → `IsRegistered(id)` | `true` | Happy Path |
| 10 | `IsRegistered_BeforeRegister_ReturnsFalse` | `IsRegistered(999)` on fresh registry | `false` | Happy Path |
| 11 | `IsRegistered_AfterUnregister_ReturnsFalse` | Register → Unregister → `IsRegistered(id)` | `false` | Happy Path |
| 12 | `GetFilePath_RegisteredId_ReturnsCorrectPath` | `RegisterFile("data/t.db", ...)` → `GetFilePath(id)` | `"data/t.db"` | Happy Path |
| 13 | `GetFilePath_UnknownId_ThrowsKeyNotFoundException` | `GetFilePath(999)` | `Exception: KeyNotFoundException` | Error Case |
| 14 | `GetFilePath_AfterUnregister_ThrowsKeyNotFoundException` | Register → Unregister → `GetFilePath(id)` | `Exception: KeyNotFoundException` | Error Case |
| 15 | `UnregisterFile_ExistingId_Succeeds` | Register → `UnregisterFile(id)` | no exception | Happy Path |
| 16 | `UnregisterFile_UnknownId_ThrowsKeyNotFoundException` | `UnregisterFile(999)` | `Exception: KeyNotFoundException` | Error Case |
| 17 | `UnregisterFile_TwiceSameId_ThrowsOnSecondCall` | Register → Unregister → Unregister same id | `Exception` on 2nd call | Error Case |
| 18 | `RegisterAfterUnregister_NewIdAssigned` | Register → Unregister → Register same path | new `id2 != id1` (IDs not reused) | Happy Path |

---

### 💾 5. DiskManager

```csharp
// Test class: DiskManagerTests : IDisposable
// Fixture: _tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".db")
// Dispose(): if (File.Exists(_tempPath)) File.Delete(_tempPath)
// Constant: PAGE_SIZE = 4096
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `CreateFile_ValidPath_CreatesPhysicalFile` | `diskMgr.CreateFile(_tempPath)` | `File.Exists(_tempPath) == true` | Happy Path |
| 2 | `CreateFile_ValidPath_ReturnsNonNegativeFileId` | `diskMgr.CreateFile(_tempPath)` | `fileId >= 0` | Happy Path |
| 3 | `CreateFile_NullPath_ThrowsArgumentNullException` | `diskMgr.CreateFile(null)` | `Exception: ArgumentNullException` | Error Case |
| 4 | `CreateFile_InvalidPath_ThrowsIOException` | `diskMgr.CreateFile(@"Z:\invalid\nonexist\a.db")` | `Exception: IOException` | Error Case |
| 5 | `CreateFile_AlreadyExists_ThrowsIOException` | Create → Create same `_tempPath` | `Exception: IOException` on 2nd call | Error Case |
| 6 | `WritePage_ThenReadPage_DataIsIdentical` | Write `new byte[4096]{0xAB,...}` page 0 → Read page 0 | `readBuffer == writeBuffer` (SequenceEqual) | Happy Path |
| 7 | `WritePage_PageN_WritesAtCorrectOffset` | Write distinct data page 5 → Read page 5 | data matches; file offset = `5 * 4096` | Happy Path |
| 8 | `ReadPage_UnwrittenPage_ReturnsZeroBytes` | ReadPage on page 5 (never written) | `buffer == new byte[4096]` (all zeros) | Boundary Case |
| 9 | `WritePage_Page2_DoesNotCorruptPage0` | Write `[0xAA]` to page 0, `[0xBB]` to page 2 → Read page 0 | `readBuf0 == [0xAA × 4096]` | Happy Path |
| 10 | `WritePage_Page2_DoesNotCorruptPage1` | Write to page 0, 1, 2 → Read page 1 | data page 1 unchanged | Happy Path |
| 11 | `OverwritePage_SamePage_LastWriteWins` | Write `[0xAA]` page 0 → Write `[0xBB]` page 0 → Read page 0 | `[0xBB × 4096]` | Happy Path |
| 12 | `MultiplePages_AllDataConsistent` | Write 10 pages with different data → Read all | all 10 reads return correct data | Happy Path |
| 13 | `WritePage_NullData_ThrowsArgumentNullException` | `WritePage(fileId, 0, null)` | `Exception: ArgumentNullException` | Error Case |
| 14 | `WritePage_WrongBufferSize_ThrowsArgumentException` | `WritePage(fileId, 0, new byte[100])` | `Exception: ArgumentException` | Error Case |
| 15 | `ReadPage_NullBuffer_ThrowsArgumentNullException` | `ReadPage(fileId, 0, null)` | `Exception: ArgumentNullException` | Error Case |
| 16 | `ReadPage_WrongBufferSize_ThrowsArgumentException` | `ReadPage(fileId, 0, new byte[100])` | `Exception: ArgumentException` | Error Case |
| 17 | `ReadPage_NegativePageId_ThrowsArgumentOutOfRangeException` | `ReadPage(fileId, -1, new byte[4096])` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 18 | `WritePage_NegativePageId_ThrowsArgumentOutOfRangeException` | `WritePage(fileId, -1, new byte[4096])` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 19 | `WritePage_InvalidFileId_ThrowsArgumentException` | `WritePage(999, 0, data)` | `Exception: ArgumentException` | Error Case |
| 20 | `ReadPage_InvalidFileId_ThrowsArgumentException` | `ReadPage(999, 0, buf)` | `Exception: ArgumentException` | Error Case |
| 21 | `WritePage_PageZero_WritesAtStartOfFile` | Write page 0 → read raw file bytes [0..4095] | match write data | Happy Path |
| 22 | `WritePage_FullPageSize_NoDataLoss` | Write exactly 4096 bytes with pattern | ReadPage returns identical 4096 bytes | Boundary Case |

---

### 🗃️ 6. FileDescriptorManager

```csharp
// Test class: FileDescriptorManagerTests
// Mock: IDataFileRegistry or use fake file paths
// Fixture: fresh manager per test
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `GetDescriptor_ValidFileId_ReturnsNonNull` | `mgr.GetDescriptor(1)` (fileId=1 registered) | `descriptor != null` | Happy Path |
| 2 | `GetDescriptor_SameFileId_ReturnsCachedDescriptor` | `GetDescriptor(1)` × 2 | `ReferenceEquals(d1, d2) == true` | Happy Path |
| 3 | `GetDescriptor_DifferentFileIds_ReturnsDifferentDescriptors` | `GetDescriptor(1)` vs `GetDescriptor(2)` | `!ReferenceEquals(d1, d2)` | Happy Path |
| 4 | `GetDescriptor_InvalidFileId_ThrowsFileNotFoundException` | `GetDescriptor(999)` | `Exception: FileNotFoundException` | Error Case |
| 5 | `GetActiveCount_Initially_IsZero` | Fresh manager | `mgr.GetActiveCount() == 0` | Boundary Case |
| 6 | `GetActiveCount_AfterOneGet_IsOne` | `GetDescriptor(1)` | `GetActiveCount() == 1` | Happy Path |
| 7 | `GetActiveCount_AfterTwoDistinctGets_IsTwo` | `GetDescriptor(1)`, `GetDescriptor(2)` | `GetActiveCount() == 2` | Happy Path |
| 8 | `GetActiveCount_GetSameFileTwice_IsOne` | `GetDescriptor(1)` × 2 (caches) | `GetActiveCount() == 1` | Boundary Case |
| 9 | `ReleaseDescriptor_AfterGet_ActiveCountDecreases` | Get(1) → Release(d1) | `GetActiveCount() == 0` | Happy Path |
| 10 | `ReleaseDescriptor_NullDescriptor_ThrowsArgumentNullException` | `ReleaseDescriptor(null)` | `Exception: ArgumentNullException` | Error Case |
| 11 | `ReleaseDescriptor_AlreadyReleased_ThrowsInvalidOperationException` | Get → Release → Release again | `Exception: InvalidOperationException` on 2nd | Error Case |
| 12 | `GetDescriptor_AfterRelease_ReturnsNewDescriptor` | Get → Release → Get again | new valid descriptor, no exception | Happy Path |

---

### ⏱️ 7. LRUReplacer

```csharp
// Test class: LRUReplacerTests
// Fixture: new LRUReplacer(capacity: 5) unless specified
// Capacity: determines valid frame IDs [0..capacity-1]
```

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 1 | `Victim_NoUnpinnedFrames_ReturnsFalse` | Fresh replacer, `Victim(out id)` | `returns false`, `id` not meaningful | Boundary Case |
| 2 | `Victim_OneUnpinnedFrame_ReturnsTrueAndId` | `Unpin(3)`, `Victim(out id)` | `true`, `id == 3` | Happy Path |
| 3 | `Victim_MultipleUnpinned_ReturnsLRU` | `Unpin(1)`, `Unpin(2)`, `Unpin(3)`, `Victim(out id)` | `id == 1` (least recently used = oldest) | Happy Path |
| 4 | `Victim_RemovesFrameFromEvictableSet` | `Unpin(1)`, `Victim(out _)`, `Victim(out _)` | 2nd returns `false` | Happy Path |
| 5 | `Victim_AfterReAccess_SkipsRecentlyUsed` | `Unpin(1)`, `Unpin(2)`, `Unpin(1)` again → `Victim` | `id == 2` (1 was re-accessed, so 2 is LRU) | Happy Path |
| 6 | `Victim_AllPinned_ReturnsFalse` | `Pin(0)`, `Pin(1)`, `Pin(2)`, `Victim(out _)` | `false` | Happy Path |
| 7 | `Victim_SequentialCalls_CorrectOrder` | `Unpin(1)`, `Unpin(2)`, `Unpin(3)`, Victim × 3 | ids sequence: `[1, 2, 3]` | Happy Path |
| 8 | `Pin_UnpinnedFrame_RemovesFromEvictableList` | `Unpin(3)`, `Pin(3)`, `Victim(out _)` | `false` (3 was re-pinned) | Happy Path |
| 9 | `Pin_AlreadyPinnedFrame_NoException` | `Pin(3)`, `Pin(3)` | no exception | Happy Path |
| 10 | `Pin_InvalidFrameId_ThrowsArgumentOutOfRangeException` | `Pin(-1)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 11 | `Pin_FrameIdBeyondCapacity_ThrowsArgumentOutOfRangeException` | `Pin(capacity)` (e.g., 5 for cap=5) | `Exception: ArgumentOutOfRangeException` | Error Case |
| 12 | `Unpin_PinnedFrame_AddsToEvictableList` | `Pin(3)`, `Unpin(3)`, `Victim(out id)` | `true`, `id == 3` | Happy Path |
| 13 | `Unpin_AlreadyUnpinned_IsIdempotent` | `Unpin(3)` × 2, Victim × 2 | 1st=`true`, 2nd=`false` | Boundary Case |
| 14 | `Unpin_InvalidFrameId_ThrowsArgumentOutOfRangeException` | `Unpin(-1)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 15 | `Capacity_Zero_VictimAlwaysFalse` | `new LRUReplacer(0)`, `Victim(out _)` | `false` | Boundary Case |
| 16 | `Size_AfterUnpin_Increases` | `Unpin(0)` | `replacer.Size == 1` | Happy Path |
| 17 | `Size_AfterVictim_Decreases` | `Unpin(0)`, `Victim(out _)` | `replacer.Size == 0` | Happy Path |
| 18 | `InterleavePattern_LRUMaintained` | `Unpin(A)`, `Unpin(B)`, `Pin(A)`, `Unpin(A)` → `Victim` | `id == B` (B was unpinned earlier) | Happy Path |

---

### 🕐 8. ClockReplacer

```csharp
// Test class: ClockReplacerTests
// Fixture: new ClockReplacer(capacity: 3) unless specified
// Semantics: Unpin sets ref_bit=1; Victim clears ref_bit=0 before evicting
```

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 1 | `Victim_NoFrames_ReturnsFalse` | Fresh replacer, `Victim(out _)` | `false` | Boundary Case |
| 2 | `Victim_OneFrameRefBitZero_EvictsImmediately` | Frame 0 unpinned (ref=0 initially?), `Victim(out id)` | `true`, `id == 0` | Happy Path |
| 3 | `Victim_OneFrameRefBitOne_ClearsAndReturnsItself` | `Unpin(0)` (sets ref=1), `Victim(out id)` | `true`, `id == 0` after full sweep | Happy Path |
| 4 | `Victim_F0RefBit1_F1RefBit0_ReturnsF1` | F0: `Unpin` (ref=1), F1: unpinned (ref=0), `Victim(out id)` | `id == 1`; F0 ref cleared to 0 | Happy Path |
| 5 | `Victim_AllRefBitOne_FullSweepThenEvicts` | 3 frames all `Unpin`'d → `Victim(out id)` | clears all refs → evicts frame 0 (clock start) | Happy Path |
| 6 | `Victim_PinnedFrame_NotEvicted` | Frame 0 pinned (ref=0), `Victim(out _)` | clock skips frame 0, returns `false` if all pinned | Happy Path |
| 7 | `Victim_AllPinned_ReturnsFalse` | All frames pinned, `Victim(out _)` | `false` | Happy Path |
| 8 | `ClockHand_WrapAround_CorrectCycle` | 3 frames unpinned (ref=0), Victim × 3 | ids = `[0, 1, 2]` in order | Happy Path |
| 9 | `Unpin_Frame_SetsRefBitOne` | `Unpin(0)`, then check internal state | `frame[0].ref_bit == 1` (via subsequent Victim behavior) | Happy Path |
| 10 | `Pin_Frame_ExcludesFromEviction` | `Unpin(0)`, `Unpin(1)`, `Pin(0)`, `Victim(out id)` | `id == 1` (0 excluded) | Happy Path |
| 11 | `Unpin_InvalidFrameId_ThrowsArgumentOutOfRangeException` | `Unpin(-1)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 12 | `Pin_InvalidFrameId_ThrowsArgumentOutOfRangeException` | `Pin(capacity + 1)` | `Exception: ArgumentOutOfRangeException` | Error Case |

---

### 🧠 9. BufferPoolManager

```csharp
// Test class: BufferPoolManagerTests
// Setup per test:
//   var mockDisk = new Mock<IDiskManager>();
//   mockDisk.Setup(d => d.ReadPage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte[]>()))
//           .Callback<int,int,byte[]>((fid, pid, buf) => Array.Clear(buf, 0, buf.Length));
//   var replacer = new LRUReplacer(poolSize);
//   var bpm = new BufferPoolManager(poolSize, mockDisk.Object, replacer);
```

#### 9A. FetchPage

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 1 | `FetchPage_CacheHit_ReturnsSamePageObject` | Fetch `pid=5` → Fetch `pid=5` again | `ReferenceEquals(p1, p2) == true` | Happy Path |
| 2 | `FetchPage_CacheHit_ReadPageCalledOnlyOnce` | Fetch `pid=5` × 2 | `Side Effect: mockDisk.Verify(d => d.ReadPage(..., 5, ...), Times.Once)` | Happy Path |
| 3 | `FetchPage_CacheHit_PinCountIncrements` | Fetch `pid=5` × 2 | `page.PinCount == 2` | Happy Path |
| 4 | `FetchPage_CacheMiss_FreeFrame_CallsReadPage` | poolSize=3, Fetch `pid=10` (cold) | `Side Effect: ReadPage(_, 10, _) called once` | Happy Path |
| 5 | `FetchPage_CacheMiss_FreeFrame_ReturnsNonNull` | poolSize=3, Fetch `pid=10` (cold) | `page != null` | Happy Path |
| 6 | `FetchPage_CacheMiss_FreeFrame_PinCountIsOne` | Fetch `pid=10` cold | `page.PinCount == 1` | Happy Path |
| 7 | `FetchPage_CacheMiss_FreeFrame_UpdatesPageTable` | Fetch `pid=10` | internal page table contains key `10` | Happy Path |
| 8 | `FetchPage_CacheMiss_Eviction_CleanFrame_NoWritePage` | poolSize=1, Fetch pid=0 → Unpin → Fetch pid=1 | `Side Effect: mockDisk.Verify(d => d.WritePage(...), Times.Never)` | Happy Path |
| 9 | `FetchPage_CacheMiss_Eviction_DirtyFrame_WritesFirst` | poolSize=1, Fetch pid=0 → mark dirty → Unpin → Fetch pid=1 | `Side Effect: WritePage(_, 0, _) called BEFORE ReadPage(_, 1, _)` | Happy Path |
| 10 | `FetchPage_CacheMiss_Eviction_OldPageRemovedFromTable` | Evict pid=0, Fetch pid=1 | pageTable does NOT contain key `0` | Happy Path |
| 11 | `FetchPage_CacheMiss_Eviction_NewPageInTable` | Evict → Fetch pid=1 | pageTable contains key `1` | Happy Path |
| 12 | `FetchPage_NoFrameAvailable_ThrowsBufferPoolFullException` | poolSize=2, Fetch pid=0 (pinned), Fetch pid=1 (pinned), Fetch pid=2 | `Exception: BufferPoolFullException` | Error Case |
| 13 | `FetchPage_NegativePageId_ThrowsArgumentOutOfRangeException` | `FetchPage(-1)` | `Exception: ArgumentOutOfRangeException` | Error Case |

#### 9B. UnpinPage

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 14 | `UnpinPage_IsDirtyTrue_MarksFrameDirty` | Fetch pid=5 → `UnpinPage(5, true)` | `frame.IsDirty == true` | Happy Path |
| 15 | `UnpinPage_IsDirtyFalse_FrameRemainsClean` | Fetch pid=5 → `UnpinPage(5, false)` | `frame.IsDirty == false` | Happy Path |
| 16 | `UnpinPage_CleanUnpinAfterDirtyUnpin_StillDirty` | Fetch → Unpin(dirty=true) → Fetch again → Unpin(dirty=false) | `frame.IsDirty == true` (dirty not cleared by clean unpin) | Boundary Case |
| 17 | `UnpinPage_DecrementsPinCount` | Fetch (pinCount=1) → `UnpinPage(5, false)` | `page.PinCount == 0` | Happy Path |
| 18 | `UnpinPage_PinCountZero_FrameAddedToReplacer` | Fetch → Unpin | replacer.Victim() returns this frame | Happy Path |
| 19 | `UnpinPage_PinCountStillPositive_NotEvictable` | Fetch × 2 (pinCount=2) → Unpin × 1 | replacer cannot victim this frame | Happy Path |
| 20 | `UnpinPage_PageNotInPool_ThrowsKeyNotFoundException` | `UnpinPage(999, false)` | `Exception: KeyNotFoundException` | Error Case |
| 21 | `UnpinPage_PinCountAlreadyZero_ThrowsInvalidOperationException` | Fetch → Unpin → Unpin again | `Exception: InvalidOperationException` on 2nd unpin | Error Case |

#### 9C. FlushPage

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 22 | `FlushPage_DirtyPage_CallsWritePage` | Fetch → Unpin(dirty=true) → `FlushPage(5)` | `Side Effect: WritePage(_, 5, _) called once` | Happy Path |
| 23 | `FlushPage_DirtyPage_ClearsIsDirtyFlag` | Fetch → dirty → `FlushPage(5)` | `frame.IsDirty == false` | Happy Path |
| 24 | `FlushPage_CleanPage_DoesNotCallWritePage` | Fetch (clean) → `FlushPage(5)` | `Side Effect: WritePage never called` | Happy Path |
| 25 | `FlushPage_PageNotInPool_ThrowsKeyNotFoundException` | `FlushPage(999)` | `Exception: KeyNotFoundException` | Error Case |

#### 9D. FlushAllPages

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 26 | `FlushAllPages_AllDirty_WritesAll` | 3 dirty pages → `FlushAllPages()` | `Side Effect: WritePage called 3 times` | Happy Path |
| 27 | `FlushAllPages_MixedDirty_WritesOnlyDirty` | 3 pages, 2 dirty, 1 clean → `FlushAllPages()` | `Side Effect: WritePage called 2 times` | Happy Path |
| 28 | `FlushAllPages_AllClean_WritesNone` | 3 clean pages → `FlushAllPages()` | `Side Effect: WritePage never called` | Boundary Case |
| 29 | `FlushAllPages_EmptyPool_NoException` | Fresh BPM → `FlushAllPages()` | no exception | Boundary Case |
| 30 | `FlushAllPages_ClearsAllDirtyFlags` | 3 dirty → `FlushAllPages()` | all `frame.IsDirty == false` | Happy Path |

#### 9E. NewPage

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 31 | `NewPage_ReturnsNonNegativePageId` | `bpm.NewPage()` | `pageId >= 0` | Happy Path |
| 32 | `NewPage_CalledTwice_IdsAreDistinct` | `NewPage()` × 2 | `id1 != id2` | Happy Path |
| 33 | `NewPage_NewPageIsPinned` | `NewPage()` returns `id` | frame for `id` has `PinCount == 1` | Happy Path |
| 34 | `NewPage_PoolFull_CanEvict_Succeeds` | poolSize=1, Fetch → Unpin → `NewPage()` | succeeds, no exception | Happy Path |
| 35 | `NewPage_PoolFull_NoEviction_Throws` | poolSize=1, Fetch (remain pinned) → `NewPage()` | `Exception: BufferPoolFullException` | Error Case |
| 36 | `NewPage_PageAddedToPageTable` | `NewPage()` returns `id` | pageTable contains key `id` | Happy Path |

#### 9F. DeletePage

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 37 | `DeletePage_UnpinnedPage_RemovesFromPageTable` | Fetch pid=5 → Unpin → `DeletePage(5)` | pageTable does NOT contain `5` | Happy Path |
| 38 | `DeletePage_UnpinnedPage_FrameReturnedToFreeList` | poolSize=1, Fetch → Unpin → Delete → `NewPage()` | NewPage succeeds (frame reused) | Happy Path |
| 39 | `DeletePage_PinnedPage_ThrowsInvalidOperationException` | Fetch pid=5 (pinned, no unpin) → `DeletePage(5)` | `Exception: InvalidOperationException` | Error Case |
| 40 | `DeletePage_PageNotInPool_ReturnsFalseOrThrows` | `DeletePage(999)` | `false` OR `Exception` *(document which)* | Error Case |
| 41 | `DeletePage_AfterDelete_FetchLoadsFromDiskAgain` | Fetch pid=5 → Unpin → Delete → Fetch pid=5 again | `Side Effect: ReadPage(_, 5, _) called 2 times` | Happy Path |

---

### 📄 10. SlottedPage

```csharp
// Test class: SlottedPageTests
// Fixture: byte[] pageData = new byte[4096]; var page = new SlottedPage(pageData);
// Constants: PAGE_SIZE=4096, HEADER_SIZE=16, SLOT_ENTRY_SIZE=4
// FormatNewPage() called in SetUp for most tests
```

#### 10A. FormatNewPage

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 1 | `FormatNewPage_FreeSpaceEqualsPageSizeMinusHeader` | `page.FormatNewPage()` | `page.GetFreeSpace() == 4080` | Happy Path |
| 2 | `FormatNewPage_SlotCountIsZero` | `page.FormatNewPage()` | `page.SlotCount == 0` | Happy Path |
| 3 | `FormatNewPage_LsnIsZero` | `page.FormatNewPage()` | `page.Header.Lsn == 0` | Happy Path |
| 4 | `FormatNewPage_FreeSpacePointerAtEndOfPage` | `page.FormatNewPage()` | `page.Header.FreeSpacePointer == 4096` | Boundary Case |
| 5 | `FormatNewPage_ReformatExistingPage_ResetsState` | `AllocateSlotEntry(50)` → `FormatNewPage()` | `SlotCount==0`, `GetFreeSpace()==4080` | Happy Path |

#### 10B. AllocateSlotEntry

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 6 | `AllocateSlotEntry_FirstRecord_ReturnsSlotIdZero` | `page.AllocateSlotEntry(50)` | `slotId == 0` | Happy Path |
| 7 | `AllocateSlotEntry_SecondRecord_ReturnsSlotIdOne` | Alloc(50) → Alloc(30) | 2nd returns `1` | Happy Path |
| 8 | `AllocateSlotEntry_SlotCountIncrements` | Alloc × 2 | `SlotCount == 2` | Happy Path |
| 9 | `AllocateSlotEntry_FreeSpaceDecreasesByRecordPlusSlotEntry` | Initial free=4080, `AllocateSlotEntry(100)` | `GetFreeSpace() == 3976` (4080 - 100 - 4) | Happy Path |
| 10 | `AllocateSlotEntry_FirstRecordOffset_IsPageEndMinusSize` | Alloc(50) | `GetSlotOffset(0) == 4046` (4096 - 50) | Happy Path |
| 11 | `AllocateSlotEntry_SecondRecordOffset_IsBelowFirst` | Alloc(50) → Alloc(30) | `GetSlotOffset(1) == 4016` (4046 - 30) | Happy Path |
| 12 | `AllocateSlotEntry_SlotLength_MatchesAllocatedSize` | Alloc(75) | `GetSlotLength(0) == 75` | Happy Path |
| 13 | `AllocateSlotEntry_SizeOne_MinimumValidSize` | `AllocateSlotEntry(1)` | `slotId == 0`, no exception | Boundary Case |
| 14 | `AllocateSlotEntry_SizeZero_ThrowsArgumentException` | `AllocateSlotEntry(0)` | `Exception: ArgumentException` | Error Case |
| 15 | `AllocateSlotEntry_NegativeSize_ThrowsArgumentException` | `AllocateSlotEntry(-1)` | `Exception: ArgumentException` | Error Case |
| 16 | `AllocateSlotEntry_ExactlyFits_Succeeds` | Free=100, `AllocateSlotEntry(96)` (96+4=100) | success, no exception | Boundary Case |
| 17 | `AllocateSlotEntry_InsufficientSpace_ThrowsPageFullException` | Free=10, `AllocateSlotEntry(100)` | `Exception: PageFullException` | Error Case |
| 18 | `AllocateSlotEntry_OneByteTooLarge_Throws` | Free=100, `AllocateSlotEntry(97)` (97+4=101 > 100) | `Exception: PageFullException` | Boundary Case |

#### 10C. MarkSlotAsDeleted

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 19 | `MarkSlotAsDeleted_ValidSlot_LengthBecomesZero` | Alloc slot 0 → `MarkSlotAsDeleted(0)` | `GetSlotLength(0) == 0` | Happy Path |
| 20 | `MarkSlotAsDeleted_FreeSpaceNotRecoveredBeforeDefrag` | Alloc(100) (free=3976) → Delete slot 0 | `GetFreeSpace() == 3976` (NOT yet recovered) | Boundary Case |
| 21 | `MarkSlotAsDeleted_SlotCountUnchanged` | Alloc × 2 → Delete slot 0 | `SlotCount == 2` (count unchanged) | Happy Path |
| 22 | `MarkSlotAsDeleted_InvalidSlotId_ThrowsArgumentOutOfRangeException` | `MarkSlotAsDeleted(999)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 23 | `MarkSlotAsDeleted_NegativeSlotId_ThrowsArgumentOutOfRangeException` | `MarkSlotAsDeleted(-1)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 24 | `MarkSlotAsDeleted_AlreadyDeleted_IsIdempotent` | Delete slot 0 → Delete slot 0 again | no exception | Boundary Case |

#### 10D. DefragmentPage

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 25 | `DefragmentPage_EmptyPage_NoChange_NoException` | `FormatNewPage()` → `Defragment()` | FreeSpace unchanged (4080), no exception | Boundary Case |
| 26 | `DefragmentPage_NoDeletedSlots_NoChange` | Alloc × 2 → `Defragment()` | FreeSpace unchanged | Boundary Case |
| 27 | `DefragmentPage_OneDeletedSlot_FreeSpaceIncreases` | Alloc(100) × 2 → Delete slot 0 → `Defragment()` | `GetFreeSpace() increased by 100` | Happy Path |
| 28 | `DefragmentPage_AllSlotsDeleted_FreeSpaceEqualsMax` | Alloc(100) × 3 → Delete all → `Defragment()` | `GetFreeSpace() == 4080` | Boundary Case |
| 29 | `DefragmentPage_RemainingSlotOffsets_Updated` | slot0(100), slot1(50) → Delete slot0 → `Defragment()` | `GetSlotOffset(1) == 4096 - 50 == 4046` | Happy Path |
| 30 | `DefragmentPage_DataIntegrity_SurvivingRecordUnchanged` | Write `"hello"` bytes in slot1 → Delete slot0 → `Defragment()` | reading slot1 data == `"hello"` bytes | Happy Path |
| 31 | `DefragmentPage_MultipleDeletions_CorrectFreeSpace` | Alloc(50), Alloc(80), Alloc(30), Alloc(60) → Delete idx 1,3 → `Defragment()` | `FreeSpace increased by 80+60 = 140` | Happy Path |

#### 10E. Misc (GetSlotOffset, GetSlotLength, GetFreeSpace, Checksum)

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 32 | `GetSlotOffset_InvalidSlotId_ThrowsArgumentOutOfRangeException` | `GetSlotOffset(999)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 33 | `GetSlotLength_InvalidSlotId_ThrowsArgumentOutOfRangeException` | `GetSlotLength(999)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 34 | `GetFreeSpace_ReflectsCurrentStateAlways` | Alloc → Delete → Defrag sequence | FreeSpace reflects actual usable space at each step | Happy Path |
| 35 | `CalculatePageChecksum_ConsistentForSameData` | Same page data → `CalculatePageChecksum()` × 2 | `cs1 == cs2` | Happy Path |
| 36 | `CalculatePageChecksum_DifferentData_DifferentChecksum` | Page A ≠ Page B → compare checksums | `checksum(A) != checksum(B)` | Happy Path |
| 37 | `CalculatePageChecksum_EmptyPage_NonZeroChecksum` | Fresh formatted page → `CalculatePageChecksum()` | `checksum != 0` *(header bytes are nonzero)* | Boundary Case |
| 38 | `CalculatePageChecksum_AfterModification_ChecksumChanges` | Alloc → cs1 → Alloc → cs2 | `cs1 != cs2` | Happy Path |

---

### 📝 11. RecordLayoutManager

```csharp
// Test class: RecordLayoutManagerTests
// Fixture: var rlm = new RecordLayoutManager(); // stateless
// Schemas:
//   _intSchema = new Schema(["val"], [IntType.Instance])
//   _varCharSchema = new Schema(["name"], [new VarCharType(10)])
//   _mixedSchema = new Schema(["id","name","age?"], [IntType, VarCharType(50), NullableIntType])
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `SerializeRecord_SingleInt_CorrectBytes` | Schema `[INT]`, values `[42]` | `bytes == [0,0,0,42]` (big-endian) | Happy Path |
| 2 | `SerializeRecord_TwoInts_CorrectLayout` | Schema `[INT,INT]`, values `[1,2]` | `bytes == [0,0,0,1, 0,0,0,2]` | Happy Path |
| 3 | `SerializeRecord_VarChar_WritesLengthPrefix` | Schema `[VARCHAR(10)]`, values `["hello"]` | `bytes == [0,5, 'h','e','l','l','o']` | Happy Path |
| 4 | `SerializeRecord_EmptyVarChar_WritesZeroLength` | Schema `[VARCHAR(10)]`, values `[""]` | `bytes == [0,0]` | Boundary Case |
| 5 | `SerializeRecord_MaxLengthVarChar_Succeeds` | Schema `[VARCHAR(5)]`, values `["hello"]` (len=5) | no exception | Boundary Case |
| 6 | `SerializeRecord_VarCharExceedsMaxLength_ThrowsArgumentException` | Schema `[VARCHAR(5)]`, values `["toolong"]` (len=7) | `Exception: ArgumentException` | Error Case |
| 7 | `SerializeRecord_NullableField_NullValue_WritesNullBit` | Schema `[INT nullable]`, values `[null]` | null bitmap bit set | Happy Path |
| 8 | `SerializeRecord_NonNullableField_NullValue_Throws` | Schema `[INT not null]`, values `[null]` | `Exception: ArgumentException` | Error Case |
| 9 | `SerializeRecord_WrongColumnCount_Throws` | Schema 3 cols, values array length 2 | `Exception: ArgumentException` | Error Case |
| 10 | `SerializeRecord_NullValuesArray_ThrowsArgumentNullException` | `rlm.SerializeRecord(schema, null)` | `Exception: ArgumentNullException` | Error Case |
| 11 | `SerializeRecord_NullSchema_ThrowsArgumentNullException` | `rlm.SerializeRecord(null, values)` | `Exception: ArgumentNullException` | Error Case |
| 12 | `DeserializeRecord_Int_ReturnsCorrectValue` | bytes `[0,0,0,42]`, schema `[INT]` | `values[0] == 42` | Happy Path |
| 13 | `DeserializeRecord_VarChar_ReturnsCorrectString` | Serialize `"hello"` → bytes → Deserialize | `values[0] == "hello"` | Happy Path |
| 14 | `DeserializeRecord_NullField_ReturnsNull` | Serialize `[null]` (nullable) → Deserialize | `values[0] == null` | Happy Path |
| 15 | `DeserializeRecord_EmptyVarChar_ReturnsEmptyString` | Serialize `""` → Deserialize | `values[0] == ""` | Boundary Case |
| 16 | `SerializeDeserialize_RoundTrip_AllTypes` | Mixed schema `[INT, VARCHAR(50), INT?]`, values `[42, "hi", null]` → roundtrip | all values match exactly | Happy Path |
| 17 | `DeserializeRecord_CorruptedBytes_ThrowsFormatException` | Malformed byte sequence | `Exception: FormatException` | Error Case |
| 18 | `GetFieldPointer_FirstField_ReturnsZero` | Schema `[INT,INT]`, field index 0 | `offset == 0` | Happy Path |
| 19 | `GetFieldPointer_SecondIntField_ReturnsFour` | Schema `[INT,INT]`, field index 1 | `offset == 4` | Happy Path |
| 20 | `GetFieldPointer_InvalidNegativeIndex_Throws` | `GetFieldPointer(schema, -1)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 21 | `GetFieldPointer_IndexOutOfRange_Throws` | Schema 2 cols, field index 2 | `Exception: ArgumentOutOfRangeException` | Error Case |
