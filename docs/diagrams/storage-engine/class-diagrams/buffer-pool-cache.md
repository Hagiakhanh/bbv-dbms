## Group 2 — RAM Buffer Pool (Buffer Pool + Cache)
*The buffer pool acts as the main memory buffer that sits between the storage engine (DiskManager) and the higher-level access methods (TableIterator). It fetches pages from disk into memory and manages page replacement using algorithms like LRU or Clock.*

```mermaid
classDiagram
    namespace BufferPoolCache {
        class IBufferPoolManager {
            <<interface>>
            +FetchPage(pageId: int) Page
            +UnpinPage(pageId: int, isDirty: bool) bool
            +FlushPage(pageId: int) bool
            +FlushAllPages() void
            +NewPage(out pageId: int) Page
            +DeletePage(pageId: int) bool
        }
        class BufferPoolManager {
            -_frames: BufferFrame[]
            -_pageTable: Dictionary~int,int~
            -_freeList: Queue~int~
            -_poolSize: int
            -_replacer: IReplacer
            -_diskManager: IDiskManager
            -FindFrame(pageId: int) int
            -EvictFrame() int
        }
        class BufferFrame {
            +PageId: int
            +PinCount: int
            +IsDirty: bool
            +FrameId: int
            -_page: Page
            +GetPage() Page
            +Reset() void
        }
        class IReplacer {
            <<interface>>
            +Victim(out frameId: int) bool
            +Pin(frameId: int) void
            +Unpin(frameId: int) void
        }
        class LRUReplacer {
            -_lruList: LinkedList~int~
            -_lruMap: Dictionary~int,LinkedListNode~int~~
            -_capacity: int
            +Victim(out frameId: int) bool
            +Pin(frameId: int) void
            +Unpin(frameId: int) void
            +Size() int
        }
        class ClockReplacer {
            -_clockHand: int
            -_refBits: bool[]
            -_capacity: int
            +Victim(out frameId: int) bool
            +Pin(frameId: int) void
            +Unpin(frameId: int) void
        }
        class DirtyPageWriter {
            -_bpm: IBufferPoolManager
            -_intervalMs: int
            -_isRunning: bool
            +Start() void
            +Stop() void
            +FlushDirtyPages() void
        }
    }
    IBufferPoolManager <|-- BufferPoolManager
    BufferPoolManager *-- BufferFrame
    BufferPoolManager --> IReplacer : uses
    IReplacer <|-- LRUReplacer
    IReplacer <|-- ClockReplacer
    BufferPoolManager *-- DirtyPageWriter
```