## Group 5 — Storage Allocation
*Role in Sequence (Write Path): Step 2. After obtaining a Tuple, the system must query the FreeSpaceMap to find a Page with sufficient space. If none is found, the ExtentManager allocates additional space.*

```mermaid
classDiagram
    namespace StorageAllocation {
        class StorageAllocator {
            +AllocatePage(fileId: int, requiredSpace: ushort) int
            +DeallocatePage(pageId: int) void
        }
        class IFreeSpaceMap {
            <<interface>>
            +LocateFreePage(requiredSpace: ushort) int
            +UpdateFsmSpace(pageId: int, freeSpace: ushort) void
            +MarkPageFull(pageId: int) void
            +AddNewPages(pageIds: List~int~) void
        }
        class FreeSpaceMap {
            -_fsmData: byte[]
            -_totalPages: int
        }
        class ExtentManager {
            -_diskManager: IDiskManager
            +AllocateExtent(fileId: int) int
            +GetExtentPageIds(extentId: int) List~int~
            +FreeExtent(extentId: int) void
            +GetExtentCount(fileId: int) int
        }
        class SegmentManager {
            +AllocateSegment(tableId: int) int
            +FreeSegment(segmentId: int) void
            +GetSegmentExtents(segmentId: int) List~int~
        }
    }
    StorageAllocator *-- IFreeSpaceMap
    StorageAllocator *-- ExtentManager
    StorageAllocator *-- SegmentManager
    IFreeSpaceMap <|-- FreeSpaceMap
    ExtentManager --> IFreeSpaceMap : updates after expand
```