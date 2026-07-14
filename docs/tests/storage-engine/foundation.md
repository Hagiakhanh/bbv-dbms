# 🧪 Test Cases Design — TDD Full Specification (Layer 1: Foundation)
## Storage Engine DBMS · C# (.NET) · xUnit · Moq

> **TDD Principles:** Red → Green → Refactor  
> **Priority:** P0 (Foundation - Test First)

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
## 🏗️ LAYER 1 — FOUNDATION (P0)
## ═══════════════════════════════════════

---

### 📌 1. RID

```csharp
// Test class: RIDTests
// No dependencies — pure unit test
```

#### Constructor Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 1 | `Constructor_ValidValues_SetsProperties` | `new RID(pageId: 3, slotId: 7)` | `rid.PageId == 3 && rid.SlotId == 7` | Happy Path |
| 2 | `Constructor_ZeroValues_IsAllowed` | `new RID(0, 0)` | `rid.PageId == 0 && rid.SlotId == 0` | Boundary Case |
| 3 | `Constructor_MaxIntValues_DoesNotOverflow` | `new RID(int.MaxValue, int.MaxValue)` | `rid.PageId == 2147483647 && rid.SlotId == 2147483647` | Boundary Case |
| 4 | `Constructor_NegativePageId_ThrowsArgumentOutOfRangeException` | `new RID(-1, 0)` | `Exception: ArgumentOutOfRangeException` | Error Case |
| 5 | `Constructor_NegativeSlotId_ThrowsArgumentOutOfRangeException` | `new RID(0, -1)` | `Exception: ArgumentOutOfRangeException` | Error Case |

**TDD Notes:**
```
When writing tests #4 and #5 before implementation:
→ Current constructor will PASS if not validated → Red
→ Add: if (pageId < 0) throw new ArgumentOutOfRangeException(nameof(pageId)) → Green
```

#### Equality Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 6 | `Equals_SamePageAndSlot_ReturnsTrue` | `new RID(3,7).Equals(new RID(3,7))` | `true` | Happy Path |
| 7 | `Equals_DifferentPageId_ReturnsFalse` | `new RID(3,7).Equals(new RID(4,7))` | `false` | Happy Path |
| 8 | `Equals_DifferentSlotId_ReturnsFalse` | `new RID(3,7).Equals(new RID(3,8))` | `false` | Happy Path |
| 9 | `Equals_BothDifferent_ReturnsFalse` | `new RID(3,7).Equals(new RID(4,8))` | `false` | Happy Path |
| 10 | `Equals_Null_ReturnsFalse` | `new RID(3,7).Equals(null)` | `false` | Error Case |
| 11 | `Equals_DifferentType_ReturnsFalse` | `new RID(3,7).Equals("(3:7)")` | `false` | Error Case |
| 12 | `EqualityOperator_SameValues_ReturnsTrue` | `new RID(3,7) == new RID(3,7)` | `true` | Happy Path |
| 13 | `InequalityOperator_DifferentValues_ReturnsTrue` | `new RID(3,7) != new RID(4,7)` | `true` | Happy Path |

#### GetHashCode Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 14 | `GetHashCode_EqualRIDs_SameHash` | `new RID(3,7).GetHashCode()` vs `new RID(3,7).GetHashCode()` | `hash1 == hash2` | Happy Path |
| 15 | `GetHashCode_DifferentRIDs_DifferentHash` | `new RID(1,2).GetHashCode()` vs `new RID(3,4).GetHashCode()` | `hash1 != hash2` *(usually true)* | Happy Path |

#### ToString Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 16 | `ToString_ReturnsCorrectFormat` | `new RID(3, 7).ToString()` | `"(3:7)"` | Happy Path |
| 17 | `ToString_ZeroValues_ReturnsCorrectFormat` | `new RID(0, 0).ToString()` | `"(0:0)"` | Boundary Case |

#### Misc Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 18 | `Default_BothPropertiesAreZero` | `default(RID)` | `PageId == 0 && SlotId == 0` | Boundary Case |
| 19 | `UsableAsDictionaryKey_Works` | `dict[new RID(1,2)] = "a"`, then `dict[new RID(1,2)]` | `== "a"` (uses GetHashCode + Equals) | Happy Path |

---

### 📋 2. Schema

```csharp
// Test class: SchemaTests
// Dependencies: ColumnType hierarchy (IntType, VarCharType) — no mocks needed
```

#### Constructor Tests

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `Constructor_ValidNamesAndTypes_Succeeds` | `names=["id","name"]`, `types=[IntType, VarCharType(50)]` | object created, no exception | Happy Path |
| 2 | `Constructor_NullNames_ThrowsArgumentNullException` | `names=null`, `types=[IntType]` | `Exception: ArgumentNullException` param: "names" | Error Case |
| 3 | `Constructor_NullTypes_ThrowsArgumentNullException` | `names=["id"]`, `types=null` | `Exception: ArgumentNullException` param: "types" | Error Case |
| 4 | `Constructor_EmptySchema_Allowed` | `names=[]`, `types=[]` | `ColumnCount == 0`, no exception | Boundary Case |
| 5 | `Constructor_MismatchedLengths_ThrowsArgumentException` | `names=["id","name"]`, `types=[IntType]` | `Exception: ArgumentException` | Error Case |
| 6 | `Constructor_DuplicateColumnNames_ThrowsArgumentException` | `names=["id","id"]`, `types=[IntType,IntType]` | `Exception: ArgumentException` | Error Case |

#### ColumnCount Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 7 | `ColumnCount_ReturnsCorrectCount` | Schema with 3 columns | `schema.ColumnCount == 3` | Happy Path |
| 8 | `ColumnCount_EmptySchema_ReturnsZero` | Schema `([], [])` | `schema.ColumnCount == 0` | Boundary Case |

#### GetColumnIndex Tests

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 9 | `GetColumnIndex_FirstColumn_ReturnsZero` | Schema `["id","name"]`, call `GetColumnIndex("id")` | `0` | Happy Path |
| 10 | `GetColumnIndex_LastColumn_ReturnsLastIndex` | Schema `["a","b","c"]`, call `GetColumnIndex("c")` | `2` | Happy Path |
| 11 | `GetColumnIndex_MiddleColumn_ReturnsCorrectIndex` | Schema `["a","b","c"]`, call `GetColumnIndex("b")` | `1` | Happy Path |
| 12 | `GetColumnIndex_NonExistentColumn_ThrowsArgumentException` | Schema `["id"]`, call `GetColumnIndex("salary")` | `Exception: ArgumentException` | Error Case |
| 13 | `GetColumnIndex_NullName_ThrowsArgumentNullException` | Schema `["id"]`, call `GetColumnIndex(null)` | `Exception: ArgumentNullException` | Error Case |
| 14 | `GetColumnIndex_EmptyString_ThrowsArgumentException` | Schema `["id"]`, call `GetColumnIndex("")` | `Exception: ArgumentException` | Error Case |
| 15 | `GetColumnIndex_CaseSensitive_NotFound` | Schema `["Id"]`, call `GetColumnIndex("id")` | `Exception: ArgumentException` *(case-sensitive)* | Error Case |

#### IsFixedLength Tests

| # | Test Name | Input | Output | Type |
|---|-----------|-------|--------|------|
| 16 | `IsFixedLength_AllIntColumns_ReturnsTrue` | Schema `[INT, INT, INT]` | `true` | Happy Path |
| 17 | `IsFixedLength_AllVarCharColumns_ReturnsFalse` | Schema `[VARCHAR(50), VARCHAR(100)]` | `false` | Happy Path |
| 18 | `IsFixedLength_MixedColumns_ReturnsFalse` | Schema `[INT, VARCHAR(50)]` | `false` | Happy Path |
| 19 | `IsFixedLength_EmptySchema_ReturnsTrue` | Schema `([], [])` | `true` *(vacuously true)* | Boundary Case |

#### ReadOnly Collection Tests

| # | Test Name | Input | Exception | Type |
|---|-----------|-------|-----------|------|
| 20 | `ColumnNames_ReturnsReadOnlyCollection` | `schema.ColumnNames.Add("extra")` | `Exception: NotSupportedException` or compile error | Error Case |
| 21 | `ColumnTypes_ReturnsReadOnlyCollection` | `schema.ColumnTypes.Add(IntType)` | `Exception: NotSupportedException` | Error Case |

---

### 📦 3. Tuple

```csharp
// Test class: TupleTests
// No external dependencies
```

| # | Test Name | Input | Output / Exception | Type |
|---|-----------|-------|--------------------|------|
| 1 | `Constructor_ValidData_Succeeds` | `new Tuple(new byte[]{1,2,3})` | object created | Happy Path |
| 2 | `Constructor_NullData_ThrowsArgumentNullException` | `new Tuple(null)` | `Exception: ArgumentNullException` | Error Case |
| 3 | `Constructor_EmptyArray_SizeIsZero` | `new Tuple(new byte[]{})` | `tuple.Size == 0` | Boundary Case |
| 4 | `Size_ReturnsDataLength` | `new Tuple(new byte[100])` | `tuple.Size == 100` | Happy Path |
| 5 | `Size_IsReadOnly_NoPublicSetter` | *(Reflection check)* `typeof(Tuple).GetProperty("Size").SetMethod.IsPublic` | `false` | Happy Path |
| 6 | `GetData_ReturnsOriginalBytes` | `data = [0xAA, 0xBB]`, `new Tuple(data).GetData()` | `[0xAA, 0xBB]` (content equal) | Happy Path |
| 7 | `GetData_ReturnsDefensiveCopy_DifferentReference` | `var t = new Tuple(data); t.GetData() vs t.GetData()` | `!ReferenceEquals(call1, call2)` | Happy Path |
| 8 | `GetData_MutatingResult_DoesNotAffectTuple` | `var d = t.GetData(); d[0] = 0xFF; t.GetData()[0]` | original value (not 0xFF) | Happy Path |
| 9 | `Rid_DefaultValue_IsZeroZero` | `new Tuple(data).Rid` | `Rid == new RID(0,0)` | Boundary Case |
| 10 | `Rid_CanBeAssigned` | `tuple.Rid = new RID(5,2)` | `tuple.Rid == new RID(5,2)` | Happy Path |
| 11 | `Rid_CanBeReassigned` | Set `RID(1,1)` → set `RID(5,2)` | `tuple.Rid == new RID(5,2)` | Happy Path |
