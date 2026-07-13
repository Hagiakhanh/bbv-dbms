## Group 8 — Access Methods
*Role in Sequence (Read Path): The highest layer. The TableScanCoordinator selects the appropriate type of Iterator (sequential scan vs. index scan) and returns the Tuples back to the Query Execution engine.*

```mermaid
classDiagram
    namespace AccessMethods {
        class TableScanCoordinator {
            -_indexManager: IndexManager
            +GetIterator(predicate: ScanPredicate) IRowIterator
            -CanUseIndex(predicate: ScanPredicate) bool
        }
        class ScanPredicate {
            +TableId: int
            +ColumnName: string
            +Operator: CompareOp
            +Value: object
        }
        class IRowIterator {
            <<interface>>
            +Open() void
            +Next() Tuple
            +Close() void
            +HasNext() bool
        }
        class TableIterator {
            -_tableHeap: TableHeap
            -_currentPageId: int
            -_currentSlotId: int
        }
        class IndexScanIterator {
            -_index: IIndex
            -_startKey: object
            -_endKey: object
        }
        class BPlusTreeIterator {
            -_currentLeafPage: BPlusTreeLeafPage
            -_currentSlot: int
            +HasNext() bool
            +NextKey() Tuple
        }
    }
    TableScanCoordinator --> IRowIterator : Coordinates
    TableScanCoordinator --> ScanPredicate : receives
    IRowIterator <|-- TableIterator
    IRowIterator <|-- IndexScanIterator
    IndexScanIterator --> BPlusTreeIterator : uses
```
