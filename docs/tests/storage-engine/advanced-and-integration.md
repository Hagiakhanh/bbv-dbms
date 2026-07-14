# 🧪 Test Cases Design — TDD Full Specification (Layer 3: Advanced & Integration)
## Storage Engine DBMS · C# (.NET) · xUnit · Moq

> **Priority:** P4-P5 (Advanced Components) → P6 (Integration Tests)

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
## 🚀 LAYER 3 — ADVANCED COMPONENTS
## ═══════════════════════════════════════

---

### 🗺️ 12. FreeSpaceMap

```csharp
// Test class: FreeSpaceMapTests
// Fixture: var fsm = new FreeSpaceMap(totalPages: 5); // pre-init 5 pages all free
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `LocateFreePage_SingleFreePageSufficient_ReturnsPageId` | 1 page with free=500, `LocateFreePage(100)` | `pageId == 0` | Happy Path |
| 2 | `LocateFreePage_MultiplePages_ReturnsFirstWithEnoughSpace` | Pages: [full, free=300, free=200], `LocateFreePage(100)` | `pageId == 1` | Happy Path |
| 3 | `LocateFreePage_ExactSpaceMatch_ReturnsPage` | Page free=100, `LocateFreePage(100)` | `pageId == 0` | Boundary Case |
| 4 | `LocateFreePage_OneByteLess_NotReturned` | Page free=99, `LocateFreePage(100)` | NOT `0`; returns next or -1 | Boundary Case |
| 5 | `LocateFreePage_AllPagesFull_ReturnsNegativeOne` | All pages full (free=0), `LocateFreePage(1)` | `-1` | Happy Path |
| 6 | `LocateFreePage_NoPages_ReturnsNegativeOne` | `new FreeSpaceMap(0)`, `LocateFreePage(1)` | `-1` | Boundary Case |
| 7 | `LocateFreePage_NeededZero_ThrowsArgumentException` | `LocateFreePage(0)` | `Exception: ArgumentException` | Error Case |
| 8 | `LocateFreePage_NeededNegative_ThrowsArgumentException` | `LocateFreePage(-1)` | `Exception: ArgumentException` | Error Case |
| 9 | `UpdateFsmSpace_UpdatesCorrectPage` | `UpdateFsmSpace(2, 200)`, `LocateFreePage(150)` | `pageId == 2` | Happy Path |
| 10 | `UpdateFsmSpace_ToZero_TreatedAsFull` | `UpdateFsmSpace(0, 0)`, `LocateFreePage(1)` | does NOT return `0` | Happy Path |
| 11 | `UpdateFsmSpace_InvalidNegativePageId_Throws` | `UpdateFsmSpace(-1, 100)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 12 | `UpdateFsmSpace_PageIdBeyondTotal_Throws` | `UpdateFsmSpace(totalPages, 100)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 13 | `UpdateFsmSpace_NegativeFreeBytes_Throws` | `UpdateFsmSpace(0, -1)` | `Exception: ArgumentException` | Error Case |
| 14 | `MarkPageFull_PageNotReturnedByLocate` | `MarkPageFull(1)`, `LocateFreePage(1)` | result does NOT equal `1` | Happy Path |
| 15 | `MarkPageFull_InvalidPageId_Throws` | `MarkPageFull(-1)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 16 | `MarkPageFull_ThenUpdateFsmSpace_PageAccessibleAgain` | `MarkPageFull(1)` → `UpdateFsmSpace(1, 200)` → `LocateFreePage(100)` | returns `1` (recovered) | Happy Path |
| 17 | `AddNewPages_IncreasesCapacity` | `new FreeSpaceMap(2)` → `AddNewPages(3)` → `LocateFreePage(1)` | can return 2, 3, or 4 | Happy Path |
| 18 | `AddNewPages_ZeroCount_NoChange` | `AddNewPages(0)` | totalPages unchanged | Boundary Case |
| 19 | `AddNewPages_NegativeCount_Throws` | `AddNewPages(-1)` | `Exception: ArgumentException` | Error Case |

---

### 📝 13. WalBuffer

```csharp
// Test class: WalBufferTests
// Fixture: var walBuf = new WalBuffer(capacityBytes: 1024);
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `Write_SingleRecord_BufferNotEmpty` | `walBuf.Write(record)` | internal size > 0 | Happy Path |
| 2 | `Write_MultipleRecords_OrderPreserved` | Write A, B, C → `Read()` | returns [A, B, C] in order | Happy Path |
| 3 | `Write_UntilCapacity_IsFull` | Write records until total bytes == capacity | `walBuf.IsFull == true` | Boundary Case |
| 4 | `Write_WhenFull_ThrowsBufferOverflowException` | Write beyond capacity | `Exception: BufferOverflowException` | Error Case |
| 5 | `Write_NullRecord_ThrowsArgumentNullException` | `walBuf.Write(null)` | `Exception: ArgumentNullException` | Error Case |
| 6 | `IsFull_Initially_ReturnsFalse` | Fresh buffer | `walBuf.IsFull == false` | Boundary Case |
| 7 | `GetCurrentLsn_Initially_ReturnsZero` | Fresh buffer | `walBuf.GetCurrentLsn() == 0` | Boundary Case |
| 8 | `GetCurrentLsn_AfterWrite_ReturnsLatestLsn` | Write record with `LSN=5` | `GetCurrentLsn() == 5` | Happy Path |
| 9 | `Read_EmptyBuffer_ReturnsEmptyList` | Fresh buffer → `Read()` | `records.Count == 0` | Boundary Case |
| 10 | `Read_DoesNotClearBuffer` | Write 3 → `Read()` × 2 | 2nd read returns same 3 records | Boundary Case |
| 11 | `Flush_ClearsAllRecords` | Write 5 → `Flush()` → `Read()` | `records.Count == 0` | Happy Path |
| 12 | `Flush_ResetsIsFull` | Fill buffer → `Flush()` | `walBuf.IsFull == false` | Happy Path |
| 13 | `Flush_AfterFlush_CanWriteAgain` | Fill → Flush → Write 1 record | no exception | Happy Path |
| 14 | `GetCurrentLsn_AfterFlush_LsnPreserved` | Write LSN=10 → `Flush()` | `GetCurrentLsn() == 10` (LSN not reset) | Boundary Case |

---

### 📝 14. WalWriter

```csharp
// Test class: WalWriterTests
// Mock: mockBuffer = new Mock<IWalBuffer>(), mockDisk = new Mock<IDiskManager>()
// Fixture: var writer = new WalWriter(mockBuffer.Object, mockDisk.Object)
```

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 1 | `AppendLog_ReturnsNonNegativeLsn` | `writer.AppendLog(record)` | `lsn >= 0` | Happy Path |
| 2 | `AppendLog_LsnIsMonotonicallyIncreasing` | `AppendLog(r1)`, `AppendLog(r2)`, `AppendLog(r3)` | `lsn1 < lsn2 < lsn3` | Happy Path |
| 3 | `AppendLog_WritesToBuffer` | `AppendLog(record)` | `Side Effect: mockBuffer.Verify(b => b.Write(record), Times.Once)` | Happy Path |
| 4 | `AppendLog_NullRecord_ThrowsArgumentNullException` | `AppendLog(null)` | `Exception: ArgumentNullException` | Error Case |
| 5 | `AppendLog_BufferFull_AutoFlushesAndContinues` | Fill buffer → `AppendLog(nextRecord)` | auto-flushes buffer, then writes, no exception | Boundary Case |
| 6 | `AppendLog_LsnContinuesAfterAutoFlush` | Writes to LSN=9 → auto-flush → AppendLog | new LSN == 10 (monotonic continuation) | Boundary Case |
| 7 | `FlushBuffer_EmptyBuffer_NoDiskWrite` | Fresh writer → `FlushBuffer()` | `Side Effect: mockDisk.Verify(d => d.WritePage(...), Times.Never)` | Boundary Case |
| 8 | `FlushBuffer_WithData_CallsDiskWrite` | `AppendLog(record)` → `FlushBuffer()` | `Side Effect: WritePage called once` | Happy Path |
| 9 | `FlushBuffer_ClearsBuffer_SecondFlushNoWrite` | `AppendLog` → `FlushBuffer()` → `FlushBuffer()` | 2nd flush: `Side Effect: no WritePage call` | Happy Path |
| 10 | `EnforceWalRule_PageLsnBelowCurrentFlushedLsn_NoFlush` | Flushed LSN=10, `EnforceWalRule(pageLsn: 5)` | `Side Effect: FlushBuffer NOT called` | Boundary Case |
| 11 | `EnforceWalRule_PageLsnEqualUnflushedLsn_FlushesBuffer` | Unflushed LSN=10, `EnforceWalRule(pageLsn: 10)` | `Side Effect: FlushBuffer triggered` | Happy Path |
| 12 | `EnforceWalRule_PageLsnAboveCurrentLsn_ThrowsException` | Current LSN=5, `EnforceWalRule(pageLsn: 10)` | `Exception: InvalidOperationException` | Error Case |

---

### 📑 15. BPlusTreeLeafPage

```csharp
// Test class: BPlusTreeLeafPageTests
// Fixture: int maxSize = 4; var leafPage = new BPlusTreeLeafPage(maxSize);
// Key type: int; Value type: RID
```

#### Lookup

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 1 | `Lookup_EmptyPage_ReturnsNull` | Fresh page, `Lookup(10)` | `null` | Boundary Case |
| 2 | `Lookup_ExistingKey_ReturnsCorrectRID` | `Insert(10, RID(1,2))` → `Lookup(10)` | `RID(1,2)` | Happy Path |
| 3 | `Lookup_NonExistingKey_ReturnsNull` | Insert keys [5,10,15], `Lookup(7)` | `null` | Happy Path |
| 4 | `Lookup_FirstKey_ReturnsCorrectRID` | Insert `(5, R1)`, `(10, R2)` → `Lookup(5)` | `R1` | Happy Path |
| 5 | `Lookup_LastKey_ReturnsCorrectRID` | Insert [5,10,15] → `Lookup(15)` | correct RID | Happy Path |
| 6 | `Lookup_KeySmallerThanAllKeys_ReturnsNull` | Keys [5,10,15], `Lookup(3)` | `null` | Boundary Case |
| 7 | `Lookup_KeyLargerThanAllKeys_ReturnsNull` | Keys [5,10,15], `Lookup(20)` | `null` | Boundary Case |

#### Insert

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 8 | `Insert_FirstKey_SizeIsOne` | `page.Insert(10, rid)` | `page.Size == 1` | Happy Path |
| 9 | `Insert_MultipleKeys_SizeIncrements` | Insert 3 keys | `page.Size == 3` | Happy Path |
| 10 | `Insert_KeysStoredSorted` | Insert 15, 5, 10 | internal order = [5, 10, 15] | Happy Path |
| 11 | `Insert_DuplicateKey_ThrowsDuplicateKeyException` | `Insert(10, r1)` → `Insert(10, r2)` | `Exception: DuplicateKeyException` | Error Case |
| 12 | `Insert_WhenAtMaxSize_ThrowsPageFullException` | Insert maxSize=4 keys → Insert 5th | `Exception: PageFullException` | Error Case |
| 13 | `Insert_KeyZero_IsAllowed` | `page.Insert(0, rid)` | `page.Size == 1`, no exception | Boundary Case |
| 14 | `Insert_NegativeKey_IsAllowed` | `page.Insert(-5, rid)` | `Lookup(-5) != null` | Boundary Case |

#### Split

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 15 | `Split_OriginalHalfSize` | maxSize=4, insert [1,2,3,4] → `Split(out newPage, out promoted)` | `page.Size == 2` | Happy Path |
| 16 | `Split_NewPageHalfSize` | maxSize=4, [1,2,3,4] → Split | `newPage.Size == 2` | Happy Path |
| 17 | `Split_OriginalKeepsSmallerKeys` | [1,2,3,4] → Split | original has keys [1,2] | Happy Path |
| 18 | `Split_NewPageHasLargerKeys` | [1,2,3,4] → Split | newPage has keys [3,4] | Happy Path |
| 19 | `Split_PromotedKeyIsFirstKeyOfNewPage` | [1,2,3,4] → Split | `promoted == 3` | Happy Path |
| 20 | `Split_NextPageIdLinked` | Split | `original.NextPageId == newPage.PageId` | Happy Path |
| 21 | `NextPageId_Initially_IsInvalidSentinel` | Fresh page | `page.NextPageId == -1` | Boundary Case |
| 22 | `Lookup_AfterSplit_OriginalPageCorrect` | [1,2,3,4] → Split → `Lookup(1)` on original | returns correct RID | Happy Path |
| 23 | `Lookup_AfterSplit_NewPageCorrect` | [1,2,3,4] → Split → `Lookup(3)` on newPage | returns correct RID | Happy Path |

---

### 🔄 16. TableIterator

```csharp
// Test class: TableIteratorTests
// Mock: mockBPM = new Mock<IBufferPoolManager>()
// Setup helpers:
//   CreateMockPage(pageId, slots) — returns mock IPage with specific slots
//   For empty table: mockBPM returns first page with 0 slots
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `Open_ValidTable_NoException` | `iter.Open()` | no exception | Happy Path |
| 2 | `Open_AlreadyOpen_ThrowsInvalidOperationException` | `Open()` → `Open()` | `Exception: InvalidOperationException` | Error Case |
| 3 | `HasNext_EmptyTable_ReturnsFalse` | Empty table → `Open()` → `HasNext()` | `false` | Happy Path |
| 4 | `HasNext_TableWithRecords_ReturnsTrue` | Table with 1 record → `Open()` → `HasNext()` | `true` | Happy Path |
| 5 | `HasNext_AfterAllRecordsExhausted_ReturnsFalse` | Iterate all → `HasNext()` | `false` | Happy Path |
| 6 | `HasNext_CalledMultipleTimes_SameResult` | `HasNext()` × 3 without calling `Next()` | `true` each time (idempotent) | Boundary Case |
| 7 | `HasNext_BeforeOpen_ThrowsInvalidOperationException` | `HasNext()` before `Open()` | `Exception: InvalidOperationException` | Error Case |
| 8 | `Next_SingleRecord_ReturnsCorrectTuple` | Table with 1 tuple (data=`[1,2,3]`) | returned tuple data == `[1,2,3]` | Happy Path |
| 9 | `Next_MultipleRecords_ReturnsInOrder` | Table with 3 records | 3 tuples returned in order | Happy Path |
| 10 | `Next_SpansMultiplePages_AllRecordsReturned` | 2 pages × 2 valid records each | total 4 tuples returned | Happy Path |
| 11 | `Next_SkipsDeletedSlots` | Page: slot 0 (deleted, length=0), slot 1 (valid) | only 1 tuple returned | Happy Path |
| 12 | `Next_WhenHasNextFalse_ThrowsInvalidOperationException` | Exhaust all records → `Next()` | `Exception: InvalidOperationException` | Error Case |
| 13 | `Next_BeforeOpen_ThrowsInvalidOperationException` | `Next()` before `Open()` | `Exception: InvalidOperationException` | Error Case |
| 14 | `Next_ReturnedTuple_HasCorrectRID` | Record at page 3, slot 1 | `tuple.Rid == new RID(3, 1)` | Happy Path |
| 15 | `Close_ReleasesAllFetchedFrames` | Open → iterate all pages → `Close()` | `Side Effect: mockBPM.Verify(b => b.UnpinPage(pageId, false), ...)` for each page | Happy Path |
| 16 | `Close_BeforeOpen_NoException` | `Close()` before `Open()` | no exception | Boundary Case |
| 17 | `Close_AfterClose_IsIdempotent` | `Close()` × 2 | no exception on 2nd | Boundary Case |
| 18 | `Reopen_AfterClose_RestartsFromBeginning` | Open → read all → Close → Open → read all | same records from beginning | Happy Path |

---

## ═══════════════════════════════════════
## 🔗 INTEGRATION TESTS (P6 — Last to run)
## ═══════════════════════════════════════

---

### 17. PageFetchingCacheMissTests

```csharp
// Test class: PageFetchingCacheMissTests
// Setup: poolSize=1, real LRUReplacer, Mock<IDiskManager>
// Purpose: verify BPM cache eviction behavior end-to-end
```

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 1 | `CacheMiss_CleanEviction_NoWritePageCall` | Pool=1, fetch pid=0 (clean) → Unpin → fetch pid=1 | `Side Effect: WritePage never called` | Happy Path |
| 2 | `CacheMiss_DirtyEviction_WriteBeforeRead` | Pool=1, fetch pid=0 → mark dirty → Unpin → fetch pid=1 | `Side Effect: WritePage(_, 0, _)` called BEFORE `ReadPage(_, 1, _)` | Happy Path |
| 3 | `CacheMiss_AfterFetch_PageTableContainsNewPage` | Pool=1, fetch pid=0 → Unpin → fetch pid=5 (miss) | pageTable contains key `5` | Happy Path |
| 4 | `CacheMiss_AfterEviction_OldPageRemovedFromTable` | Pool=1, fetch pid=0 → Unpin → fetch pid=1 (evicts 0) | pageTable does NOT contain `0` | Happy Path |
| 5 | `CacheMiss_AfterFetch_PinCountIsOne` | Fetch pid=5 (miss) | `page.PinCount == 1` | Happy Path |
| 6 | `CacheMiss_ReplacerPinCalled` | Fetch pid=5 (miss, free frame used) | `replacer.Pin(frameId)` was called | Happy Path |
| 7 | `CacheMiss_MultipleConsecutive_AllLoaded` | Pool=3, fetch 4 pages with intervening unpins | 4 `ReadPage` calls total, no exception | Happy Path |

---

### 18. InsertRecordTests

```csharp
// Test class: InsertRecordTests
// Setup: Full stack with mocks:
//   mockBPM = new Mock<IBufferPoolManager>()
//   mockWalWriter = new Mock<IWalWriter>()
//   mockFsm = new Mock<IFreeSpaceMap>()
//   mockExtentMgr = new Mock<IExtentManager>()
//   var insertOp = new InsertOperation(mockBPM.Object, mockWalWriter.Object, mockFsm.Object, mockExtentMgr.Object)
// Mock FSM returns pageId=3 by default
```

| # | Test Name | Input | Output / Side Effect | Type |
|---|-----------|-------|----------------------|------|
| 1 | `InsertTuple_WalWrittenBeforePageModified` | `insertOp.InsertTuple(tuple)` | `Side Effect: AppendLog` called BEFORE `AllocateSlotEntry` | Happy Path |
| 2 | `InsertTuple_ReturnsValidRID` | Insert tuple into page 3 | returned `RID.PageId == 3`, `RID.SlotId >= 0` | Happy Path |
| 3 | `InsertTuple_FirstInsert_SlotIdIsZero` | Insert into empty page | `rid.SlotId == 0` | Happy Path |
| 4 | `InsertTuple_SecondInsert_SamePage_SlotIdIsOne` | Insert 2 tuples, same page (enough space) | 2nd `rid.SlotId == 1` | Happy Path |
| 5 | `InsertTuple_FreeSpaceMapUpdatedAfterInsert` | Insert 100-byte tuple | `Side Effect: mockFsm.Verify(f => f.UpdateFsmSpace(pageId, newFree), Times.Once)` | Happy Path |
| 6 | `InsertTuple_UnpinCalledWithDirtyTrue` | Insert completes | `Side Effect: mockBPM.Verify(b => b.UnpinPage(pageId, true), Times.Once)` | Happy Path |
| 7 | `InsertTuple_FsmReturnsMinus1_AllocatesNewExtent` | `mockFsm.Setup(...).Returns(-1)` → `InsertTuple(tuple)` | `Side Effect: mockExtentMgr.Verify(e => e.AllocateExtent(), Times.Once)` | Happy Path |
| 8 | `InsertTuple_TwoInserts_DifferentRIDs` | Insert × 2 | `rid1 != rid2` | Happy Path |
| 9 | `InsertTuple_NullTuple_ThrowsArgumentNullException` | `insertOp.InsertTuple(null)` | `Exception: ArgumentNullException` | Error Case |
| 10 | `InsertTuple_TupleLargerThanPage_ThrowsRecordTooLargeException` | Tuple size > PAGE_SIZE | `Exception: RecordTooLargeException` | Error Case |
