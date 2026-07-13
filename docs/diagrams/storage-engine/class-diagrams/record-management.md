## Group 4 — Record Management
*Role in Sequence (Write Path): This is the initial step. The Transaction Manager calls SerializeRecord() to convert C# data into a binary Tuple before inserting it into a Page.*

```mermaid
classDiagram
    namespace RecordManagement {
        class IRecordLayoutManager {
            <<interface>>
            +SerializeRecord(fields: object[], schema: Schema) Tuple
            +DeserializeRecord(tuple: Tuple, schema: Schema) object[]
            +GetFieldPointer(tuple: Tuple, fieldIndex: int, schema: Schema) int
        }
        class RecordLayoutManager
        class TableHeap {
            -_bpm: IBufferPoolManager
            -_freeSpaceMap: IFreeSpaceMap
            -_recordLayout: IRecordLayoutManager
            +TableId: int
            +FirstPageId: int
            +InsertTuple(tuple: Tuple, out rid: RID) bool
            +GetTuple(rid: RID) Tuple
            +UpdateTuple(rid: RID, newTuple: Tuple) bool
            +DeleteTuple(rid: RID) bool
            -FindPageWithSpace(size: int) int
        }
        class Tuple {
            -_data: byte[]
            +Rid: RID
            +Size: int
            +GetData() byte[]
        }
        class Schema {
            +ColumnTypes: List~ColumnType~
            +ColumnNames: List~string~
            +GetColumnIndex(name: string) int
            +IsFixedLength(colIndex: int) bool
        }
        class VariableLengthDataManager {
            +SerializeVarChar(value: string, maxLen: int) byte[]
            +DeserializeVarChar(data: byte[], offset: int) string
            +GetVarCharLength(data: byte[], offset: int) int
        }
    }
    IRecordLayoutManager <|-- RecordLayoutManager
    IRecordLayoutManager --> Tuple : Creates/Uses
    IRecordLayoutManager --> Schema : Reads
    TableHeap --> IRecordLayoutManager : uses
    RecordLayoutManager --> VariableLengthDataManager : uses for VARCHAR
```