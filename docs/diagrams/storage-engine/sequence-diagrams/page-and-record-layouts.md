# RecordLayoutManager — Serialize & Deserialize

Context: `RecordLayoutManager` is the translation layer between **logical C# objects** and **binary page data**. This diagram provides **complete field-level encoding details** for all `DbType` values, the null bitmap, and the round-trip deserialization path. This is the exact flow exercised by `RecordLayoutManagerTests`.

---

## Binary Record Format on Page

```
Null Bitmap (ceil(N/8) bytes, one bit per nullable column)
│
├─── Fixed-Length Fields (in column order):
│     INT        →  4 bytes  (big-endian)
│     BIGINT     →  8 bytes  (big-endian)
│     FLOAT      →  8 bytes  (IEEE 754 double)
│     BOOLEAN    →  1 byte   (0x00 or 0x01)
│     DATETIME   →  8 bytes  (ticks, int64 big-endian)
│
└─── Variable-Length Fields (after all fixed fields):
      VARCHAR(N) →  [2-byte length prefix] + [UTF-8 bytes]
      CHAR(N)    →  [N bytes, null-padded]
      BLOB       →  [2-byte length prefix] + [raw bytes]
```

---

## Part A — SerializeRecord

```mermaid
sequenceDiagram
    autonumber
    actor Caller as InsertOperation / Test
    participant RLM as RecordLayoutManager
    participant VLD as VariableLengthDataManager
    participant Schema as Schema
    participant Tuple as Tuple

    Caller->>RLM: Encode row data [ SerializeRecord(schema, values[]) ]

    alt schema is null
        RLM-->>Caller: Throw ArgumentNullException (param: "schema")
    else values is null
        RLM-->>Caller: Throw ArgumentNullException (param: "values")
    else values.Length != schema.ColumnCount
        RLM-->>Caller: Throw ArgumentException ("column count mismatch")
    end

    RLM->>Schema: Get column count and types
    Schema-->>RLM: ColumnCount=3, ColumnTypes=[INT, VARCHAR(50), INT?]

    Note over RLM: Phase 1 — Write Null Bitmap<br/>For each nullable column, check if value is null.<br/>Bit i = 1 means column i is NULL.<br/>Bitmap size = ceil(3/8) = 1 byte<br/>Example: col2 is null → bitmap = 0b00000100

    loop Phase 2 — Fixed-Length columns (first pass)
        RLM->>RLM: For each column where ColumnType.IsFixedLength == true

        alt value is null AND column IsNullable
            Note over RLM: Write zero bytes (null bitmap already set)
        else value is null AND NOT nullable
            RLM-->>Caller: Throw ArgumentException ("null not allowed for non-nullable column")
        else DbType.Int
            Note over RLM: Write value as 4-byte big-endian int32
        else DbType.BigInt
            Note over RLM: Write value as 8-byte big-endian int64
        else DbType.Float
            Note over RLM: Write value as 8-byte IEEE 754 double
        else DbType.Boolean
            Note over RLM: Write 0x00 (false) or 0x01 (true)
        else DbType.DateTime
            Note over RLM: Write DateTime.Ticks as 8-byte int64
        end
    end

    loop Phase 3 — Variable-Length columns (second pass)
        RLM->>RLM: For each column where ColumnType.IsFixedLength == false

        alt DbType.VarChar or DbType.Char
            RLM->>VLD: Encode string [ SerializeVarChar(value, maxLen) ]

            alt value.Length > maxLen
                VLD-->>RLM: Throw ArgumentException ("exceeds max length")
            else valid
                VLD-->>RLM: Return [2-byte length][UTF-8 bytes]
                Note over RLM: Append to buffer: [0x00, 0x05, 'h','e','l','l','o']
            end

        else DbType.Blob
            Note over RLM: Append [2-byte length][raw bytes]
        end
    end

    Note over RLM: Concatenate: NullBitmap + FixedFields + VarLengthFields → byte[]
    RLM->>Tuple: new Tuple(byte[] data)
    Tuple-->>RLM: Tuple object (Rid = RID(0,0))
    RLM-->>Caller: Return Tuple
```

---

## Part B — DeserializeRecord

```mermaid
sequenceDiagram
    autonumber
    actor Caller as Query Executor / Test
    participant RLM as RecordLayoutManager
    participant VLD as VariableLengthDataManager
    participant Schema as Schema
    participant Tuple as Tuple

    Caller->>RLM: Decode binary data [ DeserializeRecord(tuple, schema) ]

    RLM->>Tuple: Get raw bytes [ tuple.GetData() ]
    Note over Tuple: Returns defensive copy of internal byte[]
    Tuple-->>RLM: byte[] data

    Note over RLM: Phase 1 — Read Null Bitmap (same byte count as serialized)

    loop Phase 2 — Fixed-Length columns
        RLM->>RLM: Read bytes at current offset, advance offset

        alt null bit is set for this column
            Note over RLM: Set values[i] = null
        else DbType.Int
            Note over RLM: Read 4 bytes big-endian → int32 value
        else DbType.VarChar, DbType.Blob
            Note over RLM: Skip (handled in phase 3)
        end
    end

    loop Phase 3 — Variable-Length columns
        alt null bit set
            Note over RLM: values[i] = null; skip 2 bytes (zero length prefix)
        else DbType.VarChar
            RLM->>VLD: Decode string [ DeserializeVarChar(data, offset) ]
            VLD-->>RLM: Return string value, advance offset by (2 + string.Length)
        end
    end

    alt malformed bytes (truncated, invalid length prefix, etc.)
        RLM-->>Caller: Throw FormatException
    end

    RLM-->>Caller: Return object[] values (matched to Schema columns)
```

---

## Part C — GetFieldPointer

```mermaid
sequenceDiagram
    autonumber
    actor Caller as Query Optimizer / Predicate Evaluator
    participant RLM as RecordLayoutManager
    participant Schema as Schema

    Note over Caller: Used to read a SINGLE field without full deserialization — for predicates (e.g., WHERE id = 5)

    Caller->>RLM: Get byte offset of field [ GetFieldPointer(schema, fieldIndex: 1) ]

    alt fieldIndex < 0
        RLM-->>Caller: Throw ArgumentOutOfRangeException
    else fieldIndex >= schema.ColumnCount
        RLM-->>Caller: Throw ArgumentOutOfRangeException
    end

    RLM->>Schema: Get ColumnTypes
    Schema-->>RLM: [INT, INT, VARCHAR(50)]

    Note over RLM: For fixed-length only schemas:<br/>  offset = NullBitmapSize + sum(size of columns 0..fieldIndex-1)<br/>  offset(field 0) = 1 + 0 = 1   (1-byte null bitmap)<br/>  offset(field 1) = 1 + 4 = 5   (INT=4 bytes)<br/>  offset(field 2) = N/A (variable — must deserialize)

    RLM-->>Caller: Return byte offset (e.g., 5 for field 1 in [INT, INT, ...] schema)
```

---

# Null Bitmap Detail

For a schema with `N` columns where some are nullable:

```
Bitmap byte count = ceil(N / 8)

Example: Schema [id INT, name VARCHAR(50) nullable, age INT nullable]
  → 3 columns → 1 byte bitmap

  Bit layout: 0b[col7][col6][col5][col4][col3][col2][col1][col0]
  
  Row [42, null, 25]:
    col0(id)=NOT null → bit 0 = 0
    col1(name)=NULL  → bit 1 = 1
    col2(age)=NOT null → bit 2 = 0
    Bitmap = 0b00000010 = 0x02
```

---

# Mapping to Test Cases

## SerializeRecord

| Test | Step |
|:-----|:-----|
| `SerializeRecord_SingleInt_CorrectBytes` | Part A: INT → 4 big-endian bytes |
| `SerializeRecord_TwoInts_CorrectLayout` | Part A: two ints concatenated |
| `SerializeRecord_VarChar_WritesLengthPrefix` | Part A: `[0x00, 0x05] + UTF-8` |
| `SerializeRecord_EmptyVarChar_WritesZeroLength` | Part A: `[0x00, 0x00]` |
| `SerializeRecord_VarCharExceedsMaxLength_ThrowsArgumentException` | Part A: VLD throws |
| `SerializeRecord_NullableField_NullValue_WritesNullBit` | Part A: null bitmap set |
| `SerializeRecord_NonNullableField_NullValue_Throws` | Part A step 9 |
| `SerializeRecord_WrongColumnCount_Throws` | Part A step 6 |
| `SerializeRecord_NullValuesArray_ThrowsArgumentNullException` | Part A step 4 |

## DeserializeRecord

| Test | Step |
|:-----|:-----|
| `DeserializeRecord_Int_ReturnsCorrectValue` | Part B: big-endian int32 |
| `DeserializeRecord_VarChar_ReturnsCorrectString` | Part B: VLD decode |
| `DeserializeRecord_NullField_ReturnsNull` | Part B: null bitmap read |
| `DeserializeRecord_CorruptedBytes_ThrowsFormatException` | Part B: malformed bytes |
| `SerializeDeserialize_RoundTrip_AllTypes` | Part A then Part B |

## GetFieldPointer

| Test | Step |
|:-----|:-----|
| `GetFieldPointer_FirstField_ReturnsZero` | Part C: offset 0 (after bitmap) |
| `GetFieldPointer_SecondIntField_ReturnsFour` | Part C: 1 (bitmap) + 4 (INT) = 5... actually offset relative to data start |
| `GetFieldPointer_InvalidNegativeIndex_Throws` | Part C step 4 |
| `GetFieldPointer_IndexOutOfRange_Throws` | Part C step 5 |
# SlottedPage — Internal Layout & Operations

Context: `SlottedPage` is the on-disk page structure used to store variable-length records. The [insert-record.md](overview/insert-record.md) overview mentions `AllocateSlotEntry()` in a single step; this diagram zooms into **all SlottedPage operations**: `FormatNewPage`, `AllocateSlotEntry`, `MarkSlotAsDeleted`, `DefragmentPage`, and checksum. This is the exact flow exercised by `SlottedPageTests`.

---

## Physical Page Layout

```
┌─────────────────────────────────────────────────────────┐  ← offset 0
│  PAGE HEADER (16 bytes)                                  │
│   Lsn (8B) | FreeSpacePointer (2B) | SlotCount (2B)    │
│   Checksum (4B)                                          │
├─────────────────────────────────────────────────────────┤  ← offset 16
│  SLOT DIRECTORY  ← grows DOWNWARD (toward high offset)  │
│   [Slot 0: Offset=4046, Length=50]  (4 bytes each)      │
│   [Slot 1: Offset=3996, Length=50]                      │
│   [Slot 2: Offset=0, Length=0]  ← DELETED (length=0)   │
│   ...                                                    │
│                                                          │
│   ~~~~~ F R E E   S P A C E   G A P ~~~~~               │
│                                                          │
│  DATA AREA  ← grows UPWARD (toward low offset)          │
│   [Record for slot 1 at offset 3996]                    │
│   [Record for slot 0 at offset 4046]                    │
└─────────────────────────────────────────────────────────┘  ← offset 4096
```

**Free Space = FreeSpacePointer − (HEADER_SIZE + SlotCount × SLOT_ENTRY_SIZE)**

---

## Part A — FormatNewPage

```mermaid
sequenceDiagram
    autonumber
    actor BPM as BufferPoolManager / Test Setup
    participant Page as SlottedPage

    BPM->>Page: Initialize a fresh page [ FormatNewPage() ]

    Note over Page: Write PAGE HEADER at offset 0:<br/>  Lsn = 0<br/>  FreeSpacePointer = 4096  (points to end of page)<br/>  SlotCount = 0<br/>  Checksum = 0 (recalculated on flush)

    Page-->>BPM: Page ready<br/>GetFreeSpace() = 4096 − 16 − (0 × 4) = 4080
```

---

## Part B — AllocateSlotEntry

```mermaid
sequenceDiagram
    autonumber
    actor InsertOp as InsertOperation
    participant Page as SlottedPage

    InsertOp->>Page: Reserve space for 50-byte record [ AllocateSlotEntry(size: 50) ]

    alt size <= 0
        Page-->>InsertOp: Throw ArgumentException ("size must be positive")
    end

    Page->>Page: Calculate required space = size + SLOT_ENTRY_SIZE (4)
    Note over Page: required = 50 + 4 = 54 bytes

    Page->>Page: Check free space [ GetFreeSpace() ]
    alt GetFreeSpace() < required
        Page-->>InsertOp: Throw PageFullException
    end

    Note over Page: Allocate in DATA AREA (grows upward from bottom):<br/>  newOffset = FreeSpacePointer − size<br/>  newOffset = 4096 − 50 = 4046

    Page->>Page: Update header: FreeSpacePointer = 4046

    Note over Page: Add slot entry in SLOT DIRECTORY:<br/>  slotId = SlotCount = 0<br/>  Write [Offset=4046, Length=50] at directory position 0<br/>  Increment SlotCount to 1

    Page-->>InsertOp: Return slotId = 0

    Note over Page: State after:<br/>  SlotCount = 1<br/>  FreeSpacePointer = 4046<br/>  GetFreeSpace() = 4046 − 16 − (1×4) = 4026

    InsertOp->>Page: Insert second record of 30 bytes [ AllocateSlotEntry(size: 30) ]
    Note over Page: newOffset = 4046 − 30 = 4016<br/>  slotId = 1<br/>  [Offset=4016, Length=30] at directory position 1<br/>  FreeSpacePointer = 4016, SlotCount = 2

    Page-->>InsertOp: Return slotId = 1
```

---

## Part C — MarkSlotAsDeleted & DefragmentPage

```mermaid
sequenceDiagram
    autonumber
    actor Exec as Delete / Update Operation
    participant Page as SlottedPage

    Note over Page: Initial state (after 3 allocations):<br/>  slot0: offset=3946, len=100  ← will be deleted<br/>  slot1: offset=3896, len=50   ← survives<br/>  slot2: offset=3836, len=60   ← will be deleted<br/>  FreeSpacePointer = 3836, SlotCount = 3<br/>  GetFreeSpace() = 3836 − 16 − 12 = 3808

    %% ─── Mark as Deleted ───
    Exec->>Page: Logical delete [ MarkSlotAsDeleted(slotId: 0) ]
    alt slotId < 0 or slotId >= SlotCount
        Page-->>Exec: Throw ArgumentOutOfRangeException
    else already deleted (length == 0)
        Note over Page: Idempotent — no exception
    end
    Page->>Page: Set slot 0 Length = 0 (sentinel for "deleted")
    Note over Page: SlotCount UNCHANGED = 3<br/>GetFreeSpace() UNCHANGED = 3808<br/>(space not yet recovered — fragmented)
    Page-->>Exec: OK

    Exec->>Page: Mark second deletion [ MarkSlotAsDeleted(slotId: 2) ]
    Page->>Page: Set slot 2 Length = 0
    Page-->>Exec: OK

    Note over Page: After 2 deletions:<br/>  slot0: offset=3946, len=0  ← deleted<br/>  slot1: offset=3896, len=50  ← alive<br/>  slot2: offset=3836, len=0   ← deleted<br/>  FreeSpacePointer still = 3836 (not moved)

    %% ─── Defragment ───
    Exec->>Page: Compact and recover space [ DefragmentPage() ]

    Note over Page: Defragmentation algorithm:<br/>  1. Collect surviving slots (len > 0): [slot1: len=50]<br/>  2. Reset FreeSpacePointer to 4096 (end of page)<br/>  3. For each survivor, compact data upward:<br/>     slot1 data moved to offset = 4096 − 50 = 4046<br/>  4. Rebuild slot directory (only survivors, renumbered)<br/>  5. SlotCount = 1 (only slot1 survives at new index 0)

    Page-->>Exec: Defragmentation complete
    Note over Page: State after defrag:<br/>  slot0 (was slot1): offset=4046, len=50<br/>  FreeSpacePointer = 4046<br/>  GetFreeSpace() = 4046 − 16 − (1×4) = 4026<br/>  Recovered = 160 bytes (100 + 60 from deleted records)
```

---

## Part D — Checksum

```mermaid
sequenceDiagram
    autonumber
    actor BPM as BufferPoolManager (on flush)
    participant Page as SlottedPage

    BPM->>Page: Compute integrity hash [ CalculatePageChecksum() ]
    Note over Page: Hash computed over entire _data[] byte array<br/>(typically CRC32 or Adler-32 of all 4096 bytes<br/> with the checksum field zeroed out during computation)
    Page-->>BPM: Return uint checksum

    Note over BPM: Store checksum in page header before writing to disk.<br/>On ReadPage, verify: recompute == stored → detect corruption.
```

---

# Free Space Formula Summary

| Metric | Formula | Example (initial) |
|:-------|:--------|:-----------------|
| `GetFreeSpace()` | `FreeSpacePointer − HEADER_SIZE − (SlotCount × 4)` | `4096 − 16 − 0 = 4080` |
| After `AllocateSlotEntry(100)` | `FreeSpacePointer` decreases by 100; SlotCount increases by 1 | `4096 − 100 − 16 − 4 = 3976` |
| After `MarkSlotAsDeleted(0)` | No change (logical only) | `3976` (unchanged) |
| After `DefragmentPage()` | Reclaims deleted record bytes | `3976 + 100 = 4076` |

---

# Mapping to Test Cases

## 10A. FormatNewPage

| Test | Step |
|:-----|:-----|
| `FormatNewPage_FreeSpaceEqualsPageSizeMinusHeader` | Part A: `GetFreeSpace() == 4080` |
| `FormatNewPage_SlotCountIsZero` | Part A: `SlotCount = 0` |
| `FormatNewPage_FreeSpacePointerAtEndOfPage` | Part A: `FreeSpacePointer = 4096` |
| `FormatNewPage_ReformatExistingPage_ResetsState` | Part A: re-format resets all |

## 10B. AllocateSlotEntry

| Test | Step |
|:-----|:-----|
| `AllocateSlotEntry_FirstRecord_ReturnsSlotIdZero` | Part B step 11 |
| `AllocateSlotEntry_FreeSpaceDecreasesByRecordPlusSlotEntry` | Part B: `4080 − 100 − 4 = 3976` |
| `AllocateSlotEntry_FirstRecordOffset_IsPageEndMinusSize` | Part B: `offset = 4096 − 50 = 4046` |
| `AllocateSlotEntry_InsufficientSpace_ThrowsPageFullException` | Part B step 5 |
| `AllocateSlotEntry_ExactlyFits_Succeeds` | Part B: boundary — exactly fits |

## 10C. MarkSlotAsDeleted

| Test | Step |
|:-----|:-----|
| `MarkSlotAsDeleted_ValidSlot_LengthBecomesZero` | Part C step 7 |
| `MarkSlotAsDeleted_FreeSpaceNotRecoveredBeforeDefrag` | Part C: space unchanged after delete |
| `MarkSlotAsDeleted_AlreadyDeleted_IsIdempotent` | Part C: idempotent |
| `MarkSlotAsDeleted_InvalidSlotId_ThrowsArgumentOutOfRangeException` | Part C step 4 |

## 10D. DefragmentPage

| Test | Step |
|:-----|:-----|
| `DefragmentPage_OneDeletedSlot_FreeSpaceIncreases` | Part C defrag steps |
| `DefragmentPage_RemainingSlotOffsets_Updated` | Part C: new offsets after compact |
| `DefragmentPage_DataIntegrity_SurvivingRecordUnchanged` | Part C: data moved correctly |
| `DefragmentPage_AllSlotsDeleted_FreeSpaceEqualsMax` | Part C: all deleted → max free |
# BPlusTreeLeafPage — Index Node Operations

Context: `BPlusTreeLeafPage` represents a **leaf node** in a B+ Tree index. Unlike `SlottedPage` which stores arbitrary binary records, a leaf page stores sorted `(key → RID)` pairs and supports **splitting** when full. This diagram details `Lookup`, `Insert`, and `Split` operations. This is the exact flow exercised by `BPlusTreeLeafPageTests`.

---

## Physical Layout of a Leaf Page

```
┌─────────────────────────────────────────────────────────────┐
│  Leaf Page Header                                           │
│   PageId | NextPageId (linked list) | KeyCount              │
├─────────────────────────────────────────────────────────────┤
│  Sorted Key-Value Array (maxSize slots)                     │
│   [Key=5,  RID=(1,0)]                                       │
│   [Key=10, RID=(1,1)]                                       │
│   [Key=15, RID=(2,3)]                                       │
│   [Key=20, RID=(3,0)]                                       │
│   ... (up to maxSize entries)                               │
└─────────────────────────────────────────────────────────────┘
         │ NextPageId
         ▼
   [Next Leaf Page]   ← leaf pages form a doubly-linked list for range scans
```

---

## Part A — Lookup (Binary Search)

```mermaid
sequenceDiagram
    autonumber
    actor BTree as BPlusTree (internal node / iterator)
    participant Leaf as BPlusTreeLeafPage

    BTree->>Leaf: Find record by key [ Lookup(key: 10) ]

    alt page is empty (Size == 0)
        Leaf-->>BTree: Return null
    end

    Note over Leaf: Binary search in sorted key array<br/>Keys: [5, 10, 15, 20]<br/>→ mid = 10 MATCH at index 1

    alt key found
        Leaf-->>BTree: Return RID at index 1 (e.g., RID(1,1))
    else key not found (key < min or key > max or gap)
        Leaf-->>BTree: Return null
    end
```

---

## Part B — Insert (Maintain Sorted Order)

```mermaid
sequenceDiagram
    autonumber
    actor BTree as BPlusTree
    participant Leaf as BPlusTreeLeafPage

    BTree->>Leaf: Insert new index entry [ Insert(key: 7, rid: RID(2,5)) ]

    alt Size >= maxSize
        Leaf-->>BTree: Throw PageFullException ("leaf page full, must split first")
    end

    Leaf->>Leaf: Binary search: find insertion position<br/>Keys: [5, 10, 15, 20] → insert at index 1

    alt key already exists at this position
        Leaf-->>BTree: Throw DuplicateKeyException ("B+ tree requires unique keys")
    end

    Note over Leaf: Shift entries right to make room:<br/>  [5, 10, 15, 20] → shift → [5, _, 10, 15, 20]<br/>  Insert at index 1: [5, 7, 10, 15, 20]

    Leaf->>Leaf: Size++ (now 5)
    Leaf-->>BTree: OK — entry inserted, page remains sorted
```

---

## Part C — Split (Page Overflow Resolution)

```mermaid
sequenceDiagram
    autonumber
    actor BTree as BPlusTree (parent node)
    participant Leaf as BPlusTreeLeafPage (original, full)
    participant NewLeaf as BPlusTreeLeafPage (newly allocated)

    Note over BTree,Leaf: Precondition: Leaf is FULL (Size == maxSize)<br/>Example: maxSize=4, keys=[1, 2, 3, 4]

    BTree->>Leaf: Split this page [ Split(out newPage, out promotedKey) ]

    Note over Leaf: Determine split point: mid = maxSize / 2 = 2

    Note over Leaf: Original keeps: keys[0..mid-1] = [1, 2]<br/>New page gets:  keys[mid..max-1] = [3, 4]

    Leaf->>NewLeaf: Allocate new leaf page
    Leaf->>NewLeaf: Copy upper half of entries [ newPage.keys = [3, 4] ]

    Note over Leaf: Truncate original: Size = 2 (keys=[1,2])
    Note over NewLeaf: NewPage: Size = 2 (keys=[3,4])

    Note over Leaf: Promoted key = first key of NEW page = 3<br/>(used by parent internal node to update routing)

    Leaf->>NewLeaf: Set linked list pointer [ original.NextPageId = newPage.PageId ]
    Note over NewLeaf: newPage.NextPageId = original's old NextPageId<br/>(maintains the doubly-linked leaf chain)

    Leaf-->>BTree: Split complete<br/>promotedKey = 3 (parent inserts this into its routing table)
    Note over BTree: Parent internal node now has two children:<br/>  left child:  [1, 2] (original page)<br/>  right child: [3, 4] (new page)<br/>  routing key: 3 (keys ≥ 3 go right)
```

---

## Part D — NextPageId (Leaf Linked List)

```mermaid
sequenceDiagram
    autonumber
    actor Iterator as BPlusTreeIterator (range scan)
    participant Leaf0 as Leaf Page 0 (keys=[5,10])
    participant Leaf1 as Leaf Page 1 (keys=[15,20,25])
    participant Leaf2 as Leaf Page 2 (keys=[30])

    Note over Iterator: Range scan: WHERE key BETWEEN 10 AND 25

    Iterator->>Leaf0: Lookup(key: 10) — start of range
    Leaf0-->>Iterator: RID(1,1) — found

    Iterator->>Leaf0: What is next leaf? [ Leaf0.NextPageId ]
    Leaf0-->>Iterator: pageId = 1

    Iterator->>Leaf1: Scan all entries on page 1
    Note over Leaf1: 15 ≤ 25 ✅ → yield RID<br/>20 ≤ 25 ✅ → yield RID<br/>25 ≤ 25 ✅ → yield RID

    Iterator->>Leaf1: NextPageId
    Leaf1-->>Iterator: pageId = 2

    Iterator->>Leaf2: Scan page 2
    Note over Leaf2: 30 > 25 ❌ — stop scan

    Note over Iterator: Initially (fresh page): NextPageId = -1 (sentinel = no next page)
```

---

# Split Decision Summary

| Condition | Action |
|:---------|:-------|
| `Size < maxSize` | `Insert()` directly (no split) |
| `Size == maxSize` | Call `Split()` FIRST, then insert into appropriate half |
| After `Split()` | Parent must insert `promotedKey` and pointer to `newPage` |

---

# Mapping to Test Cases

## Lookup

| Test | Step |
|:-----|:-----|
| `Lookup_EmptyPage_ReturnsNull` | Part A: empty page → null |
| `Lookup_ExistingKey_ReturnsCorrectRID` | Part A: binary search match |
| `Lookup_NonExistingKey_ReturnsNull` | Part A: gap → null |
| `Lookup_KeySmallerThanAllKeys_ReturnsNull` | Part A: key < min |
| `Lookup_KeyLargerThanAllKeys_ReturnsNull` | Part A: key > max |

## Insert

| Test | Step |
|:-----|:-----|
| `Insert_FirstKey_SizeIsOne` | Part B: first entry |
| `Insert_KeysStoredSorted` | Part B: sorted insertion |
| `Insert_DuplicateKey_ThrowsDuplicateKeyException` | Part B step 7 |
| `Insert_WhenAtMaxSize_ThrowsPageFullException` | Part B step 4 |
| `Insert_NegativeKey_IsAllowed` | Part B: no restriction on key values |

## Split

| Test | Step |
|:-----|:-----|
| `Split_OriginalHalfSize` | Part C: original.Size = maxSize/2 |
| `Split_NewPageHasLargerKeys` | Part C: upper half to new page |
| `Split_PromotedKeyIsFirstKeyOfNewPage` | Part C: promotedKey = new.keys[0] |
| `Split_NextPageIdLinked` | Part C: original.NextPageId = newPage.PageId |
| `NextPageId_Initially_IsInvalidSentinel` | Part D: -1 on fresh page |
| `Lookup_AfterSplit_OriginalPageCorrect` | Part C + Part A: lookup on original |
| `Lookup_AfterSplit_NewPageCorrect` | Part C + Part A: lookup on new page |
