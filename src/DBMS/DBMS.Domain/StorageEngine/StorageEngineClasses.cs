namespace DBMS.Domain.StorageEngine;

using System.Collections.Generic;
using System.IO;
using DBMS.Domain.Models;
using DBMS.Domain.DatabaseObjectManagement;

public class DiskManager
{
    public Dictionary<string, object> OpenFiles { get; set; }
    public string DataDir { get; set; }

    public void ReadPage(PageId pageId, object buffer) => throw new System.NotImplementedException();
    public void WritePage(PageId pageId, object buffer) => throw new System.NotImplementedException();
    public void AllocateFile() => throw new System.NotImplementedException();
}

public class TablespaceManager
{
    public Dictionary<string, object> Tablespaces { get; set; }

    public void CreateTablespace() => throw new System.NotImplementedException();
    public void GetTablespace() => throw new System.NotImplementedException();
    public void MapToFile() => throw new System.NotImplementedException();
}

public class PageManager
{
    public DiskManager DiskManager { get; set; }

    public void AllocatePage() => throw new System.NotImplementedException();
    public void FetchPage() => throw new System.NotImplementedException();
    public void FreePage() => throw new System.NotImplementedException();
}

public class PageFormatter
{
    public PageHeader Header { get; set; }
    public SlotArray Slots { get; set; }

    public void FormatSlottedPage() => throw new System.NotImplementedException();
    public void InsertTuple() => throw new System.NotImplementedException();
}

public class BufferPoolManager
{
    public Page[] Frames { get; set; }
    public PageTable PageTable { get; set; }
    public ReplacementPolicy Policy { get; set; }

    public Page FetchPage(PageId pageId) => throw new System.NotImplementedException();
    public void UnpinPage(PageId pageId) => throw new System.NotImplementedException();
    public void FlushPage(PageId pageId) => throw new System.NotImplementedException();
}

public class ReplacementPolicy
{
    public int FrameCount { get; set; }

    public void RecordAccess(FrameId frameId) => throw new System.NotImplementedException();
    public FrameId EvictFrame() => throw new System.NotImplementedException();
}

public class RecordManager
{
    public BufferPoolManager Bpm { get; set; }

    public RID InsertRecord(Record record) => throw new System.NotImplementedException();
    public void DeleteRecord(RID rid) => throw new System.NotImplementedException();
    public void UpdateRecord(RID rid, Record record) => throw new System.NotImplementedException();
    public Record ReadRecord(RID rid) => throw new System.NotImplementedException();
}

public class RecordLayoutManager
{
    public Schema Schema { get; set; }
    public int FixedLength { get; set; }

    public void GetFieldOffset(int colId) => throw new System.NotImplementedException();
    public void Serialize() => throw new System.NotImplementedException();
}

public class RIDGenerator
{
    public PageId CurrentPageId { get; set; }
    public int NextSlotId { get; set; }

    public void GenerateNextRID() => throw new System.NotImplementedException();
}

public class IndexManager
{
    public BufferPoolManager Bpm { get; set; }
    public MetadataCatalog Catalog { get; set; }

    public void CreateIndex() => throw new System.NotImplementedException();
    public void DropIndex() => throw new System.NotImplementedException();
    public void GetIndex() => throw new System.NotImplementedException();
}

public class BPlusTree
{
    public PageId RootPageId { get; set; }
    public int Degree { get; set; }

    public void Insert(Key key, RID rid) => throw new System.NotImplementedException();
    public void Search(Key key) => throw new System.NotImplementedException();
    public void RangeScan() => throw new System.NotImplementedException();
}

public class HashIndex
{
    public PageId DirectoryPageId { get; set; }

    public void Insert(Key key, RID rid) => throw new System.NotImplementedException();
    public void Search(Key key) => throw new System.NotImplementedException();
}

public interface AccessMethod
{
    ExecutionPlan Plan { get; set; }

    void Init();
    void Next();
    void Close();
}

public class TableScan : AccessMethod
{
    public ExecutionPlan Plan { get; set; }
    public TableId TableId { get; set; }
    public RID CurrentRid { get; set; }

    public void Init() => throw new System.NotImplementedException();
    public void Next() => throw new System.NotImplementedException();
    public void Close() => throw new System.NotImplementedException();
}

public class IndexScan : AccessMethod
{
    public ExecutionPlan Plan { get; set; }
    public IndexId IndexId { get; set; }
    public Key SearchKey { get; set; }

    public void Init() => throw new System.NotImplementedException();
    public void Next() => throw new System.NotImplementedException();
    public void Close() => throw new System.NotImplementedException();
}

public class SpaceManager
{
    public ExtentManager ExtentMgr { get; set; }

    public void AllocateSpace() => throw new System.NotImplementedException();
    public void FreeSpace() => throw new System.NotImplementedException();
}

public class ExtentManager
{
    public Dictionary<string, object> Extents { get; set; }

    public void AllocateExtent() => throw new System.NotImplementedException();
}

public class SegmentManager
{
    public Dictionary<string, object> Segments { get; set; }

    public void CreateSegment() => throw new System.NotImplementedException();
    public void GrowSegment() => throw new System.NotImplementedException();
}

public class WALManager
{
    public LogBuffer Buffer { get; set; }
    public LogWriter Writer { get; set; }

    public LSN AppendLogRecord(object record) => throw new System.NotImplementedException();
    public void FlushLog() => throw new System.NotImplementedException();
}

public class LogWriter
{
    public FileInfo LogFile { get; set; }
    public int CurrentOffset { get; set; }

    public void Write(object data) => throw new System.NotImplementedException();
    public void Fsync() => throw new System.NotImplementedException();
}

public class LSNGenerator
{
    public LSN CurrentLSN { get; set; }

    public void GetNextLSN() => throw new System.NotImplementedException();
}
