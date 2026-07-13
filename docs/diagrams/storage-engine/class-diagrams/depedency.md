# Storage Engine - Dependency Map Between 8 Groups

Instead of a giant flat diagram, this architectural map is **segmented into distinct groups (clusters)** based on the practical roles of each class across the 2 sequence diagram workflows:

- **Workflow 1 (Read path):** `TableIterator → BufferPoolManager → IReplacer → DiskManager`
- **Workflow 2 (Write path):** `RecordLayoutManager → FreeSpaceMap/ExtentManager → SlottedPage → WalWriter`

---

## Dependency Map Between 8 Groups
*Each node below is the specific class responsible for crossing the group boundary. The group it belongs to is shown in brackets.*

```mermaid
classDiagram
    class BufferPoolManager["BufferPoolManager\n[Group 2]"]
    class DiskManager["DiskManager\n[Group 1]"]
    class SlottedPage["SlottedPage\n[Group 3]"]
    class RecordLayoutManager["RecordLayoutManager\n[Group 4]"]
    class TableHeap["TableHeap\n[Group 4]"]
    class FreeSpaceMap["FreeSpaceMap\n[Group 5]"]
    class ExtentManager["ExtentManager\n[Group 5]"]
    class WalWriter["WalWriter\n[Group 6]"]
    class BPlusTreeIndex["BPlusTreeIndex\n[Group 7]"]
    class TableScanCoordinator["TableScanCoordinator\n[Group 8]"]
    class TableIterator["TableIterator\n[Group 8]"]
    class IndexScanIterator["IndexScanIterator\n[Group 8]"]

    BufferPoolManager --> DiskManager : ReadPage() / WritePage()
    BufferPoolManager --> SlottedPage : returns on FetchPage()
    TableHeap --> FreeSpaceMap : LocateFreePage()
    TableHeap --> SlottedPage : AllocateSlotEntry()
    TableHeap --> RecordLayoutManager : SerializeRecord()
    ExtentManager --> DiskManager : expand file on AllocateExtent()
    ExtentManager --> FreeSpaceMap : AddNewPages() after expand
    WalWriter --> DiskManager : WritePage() to flush WAL
    BPlusTreeIndex --> BufferPoolManager : FetchPage() for tree nodes
    TableIterator --> BufferPoolManager : FetchPage() for data pages
    TableIterator --> RecordLayoutManager : DeserializeRecord()
    IndexScanIterator --> BPlusTreeIndex : FindIndexKey() / GetRangeKeys()
    TableScanCoordinator --> TableIterator : creates for full scan
    TableScanCoordinator --> IndexScanIterator : creates for index scan
```
