namespace DBMS.Domain.StorageEngine;

using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class DiskManager
{
    private Dictionary<FileId, FileHandle> openFiles;
    private AbsolutePath dataDir;
    private FileHandle EnsureOpen(AbsolutePath path) => throw new System.NotImplementedException();
    public byte[] ReadPage(PageId id) => throw new System.NotImplementedException();
    public void WritePage(PageId id, byte[] data) => throw new System.NotImplementedException();
    public FileId AllocateFile(AbsolutePath path) => throw new System.NotImplementedException();
}

public class TablespaceManager
{
    private Dictionary<TablespaceId, Tablespace> tablespaces;
    private IMetadataCatalog catalog;
    public void CreateTablespace(string name, AbsolutePath path) => throw new System.NotImplementedException();
    public Tablespace GetTablespace(TablespaceId id) => throw new System.NotImplementedException();
    public AbsolutePath MapDatabaseToFile(DatabaseName db) => throw new System.NotImplementedException();
}

public class PageManager
{
    private DiskManager diskManager;
    private SpaceManager spaceMgr;
    public PageId AllocatePage(TableId tableId) => throw new System.NotImplementedException();
    public RawPage FetchPage(PageId id) => throw new System.NotImplementedException();
    public void FreePage(PageId id) => throw new System.NotImplementedException();
}

public class PageFormatter
{
    private PageHeader FormatHeader(RawPage page) => throw new System.NotImplementedException();
    private int ComputeFreeSpace(RawPage page) => throw new System.NotImplementedException();
    public FormattedPage FormatSlottedPage(RawPage page) => throw new System.NotImplementedException();
    public SlotId InsertTuple(FormattedPage page, byte[] tuple) => throw new System.NotImplementedException();
}

public class BufferPoolManager : IBufferPool
{
    private object[] frames;
    private object pageTable;
    private IReplacementPolicy policy;
    private DiskManager diskManager;
    
    private object FindInCache(PageId id) => throw new System.NotImplementedException();
    private object LoadFromDisk(PageId id) => throw new System.NotImplementedException();
    private void EvictIfFull() => throw new System.NotImplementedException();
    
    public object FetchPage(PageId id) => throw new System.NotImplementedException();
    public void UnpinPage(PageId id) => throw new System.NotImplementedException();
    public void FlushPage(PageId id) => throw new System.NotImplementedException();
    public void MarkDirty(PageId id) => throw new System.NotImplementedException();
}

public class LRUPolicy : IReplacementPolicy
{
    private PoolSize frameCount;
    private Dictionary<int, object> usageOrder;
    public void RecordAccess(int frameId) => throw new System.NotImplementedException();
    public int? EvictFrame() => throw new System.NotImplementedException();
    public void Pin(int frameId) => throw new System.NotImplementedException();
    public void Unpin(int frameId) => throw new System.NotImplementedException();
}

public class ClockPolicy : IReplacementPolicy
{
    private PoolSize frameCount;
    private bool[] referenceBits;
    private int clockHand;
    public void RecordAccess(int frameId) => throw new System.NotImplementedException();
    public int? EvictFrame() => throw new System.NotImplementedException();
    public void Pin(int frameId) => throw new System.NotImplementedException();
    public void Unpin(int frameId) => throw new System.NotImplementedException();
}

public class RecordManager
{
    private IBufferPool bufferPool;
    private RecordLayoutManager layoutMgr;
    private RIDGenerator ridGen;
    public RID InsertRecord(TableId tableId, byte[] data) => throw new System.NotImplementedException();
    public void DeleteRecord(RID rid) => throw new System.NotImplementedException();
    public void UpdateRecord(RID rid, byte[] data) => throw new System.NotImplementedException();
    public Record? ReadRecord(RID rid) => throw new System.NotImplementedException();
}

public class RecordLayoutManager
{
    private Schema schema;
    public int GetFieldOffset(ColId colId) => throw new System.NotImplementedException();
    public byte[] Serialize(Record record) => throw new System.NotImplementedException();
    public Record Deserialize(byte[] data) => throw new System.NotImplementedException();
}

public class RIDGenerator
{
    private PageId currentPageId;
    private int nextSlotId;
    public RID GenerateNextRID(TableId tableId) => throw new System.NotImplementedException();
}

public class IndexManager
{
    private IBufferPool bufferPool;
    private IMetadataCatalog catalog;
    private Dictionary<IndexId, IIndexStructure> indexCache;
    public IndexId CreateIndex(IndexDef def) => throw new System.NotImplementedException();
    public void DropIndex(IndexId id) => throw new System.NotImplementedException();
    public IIndexStructure GetIndex(IndexId id) => throw new System.NotImplementedException();
}

public class BPlusTree : IIndexStructure
{
    private PageId rootPageId;
    private BTreeDegree degree;
    private IBufferPool bufferPool;
    private PageId SplitNode(PageId nodeId) => throw new System.NotImplementedException();
    private void PropagateSplit(PageId child, PageId parent) => throw new System.NotImplementedException();
    private PageId FindLeaf(Key key) => throw new System.NotImplementedException();
    
    public void Insert(Key key, RID rid) => throw new System.NotImplementedException();
    public void Delete(Key key) => throw new System.NotImplementedException();
    public RID? Search(Key key) => throw new System.NotImplementedException();
    public IEnumerable<RID> RangeScan(Key fromKey, Key toKey) => throw new System.NotImplementedException();
}

public class HashIndex : IIndexStructure
{
    private PageId directoryPageId;
    private IBufferPool bufferPool;
    private PageId ResolveChain(int hash) => throw new System.NotImplementedException();
    
    public void Insert(Key key, RID rid) => throw new System.NotImplementedException();
    public void Delete(Key key) => throw new System.NotImplementedException();
    public RID? Search(Key key) => throw new System.NotImplementedException();
    public IEnumerable<RID> RangeScan(Key fromKey, Key toKey) => throw new System.NotImplementedException();
}

public class TableScan : IAccessMethod
{
    private TableId tableId;
    private RID currentRid;
    private RecordManager recordMgr;
    public void Open(ScanContext ctx) => throw new System.NotImplementedException();
    public Tuple? Next() => throw new System.NotImplementedException();
    public void Close() => throw new System.NotImplementedException();
}

public class IndexScan : IAccessMethod
{
    private IndexId indexId;
    private Key searchKey;
    private IIndexStructure index;
    public void Open(ScanContext ctx) => throw new System.NotImplementedException();
    public Tuple? Next() => throw new System.NotImplementedException();
    public void Close() => throw new System.NotImplementedException();
}

public class SpaceManager
{
    private ExtentManager extentMgr;
    public ExtentId AllocateSpace(int size) => throw new System.NotImplementedException();
    public void FreeSpace(ExtentId extent) => throw new System.NotImplementedException();
    public long AvailableSpace() => throw new System.NotImplementedException();
}

public class ExtentManager
{
    private Dictionary<ExtentId, ExtentInfo> extents;
    private SegmentManager segMgr;
    public ExtentId AllocateExtent(SegmentId segId) => throw new System.NotImplementedException();
    public void FreeExtent(ExtentId id) => throw new System.NotImplementedException();
}

public class SegmentManager
{
    private Dictionary<SegmentId, SegmentInfo> segments;
    public SegmentId CreateSegment(TableId tableId) => throw new System.NotImplementedException();
    public void GrowSegment(SegmentId id) => throw new System.NotImplementedException();
    public void DropSegment(SegmentId id) => throw new System.NotImplementedException();
}
