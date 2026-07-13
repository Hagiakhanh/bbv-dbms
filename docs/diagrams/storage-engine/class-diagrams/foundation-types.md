These structures (structs) and enumerations (enums) are referenced by almost every class across the entire system. This group MUST be defined BEFORE writing any other classes — otherwise, the compiler will trigger errors immediately.

```mermaid
classDiagram
    namespace FoundationTypes {
        class RID {
            <<struct>>
            +PageId: int
            +SlotId: int
            +RID(pageId: int, slotId: int)
            +ToString() string
            +Equals(other: RID) bool
        }
        class ColumnType {
            <<struct>>
            +DataType: DbType
            +MaxLength: int
            +IsNullable: bool
            +IsFixedLength: bool
        }
        class FileType {
            <<enum>>
            Data
            Index
            Catalog
            Log
        }
        class LogRecordType {
            <<enum>>
            Insert
            Delete
            Update
            Commit
            Abort
            Begin
            Checkpoint
        }
        class CompareOp {
            <<enum>>
            Equal
            NotEqual
            LessThan
            LessOrEqual
            GreaterThan
            GreaterOrEqual
        }
        class DbType {
            <<enum>>
            Int
            BigInt
            Float
            Char
            VarChar
            Boolean
            DateTime
            Blob
        }
    }
    ColumnType --> DbType : uses
```

> **Note:** `RID` is used by `Tuple`, `IIndex`, `BPlusTreeLeafPage`, `TableHeap`. `FileType` is used by `FileMetadata`. `LogRecordType` is used by `LogRecord`. `ColumnType` is used by `Schema`. `CompareOp` is used by `ScanPredicate`.
