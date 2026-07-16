namespace DBMS.Domain.Interfaces;

using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IBufferPool
{
    object FetchPage(PageId id);
    void UnpinPage(PageId id);
    void FlushPage(PageId id);
    void MarkDirty(PageId id);
}

public interface IWALManager
{
    LSN Append(LogRecord record);
    void Flush(LSN upToLSN);
    void TruncateBefore(LSN lsn);
}

public interface ILogWriter
{
    void Write(byte[] bytes);
    void Fsync();
    long CurrentOffset();
}

public interface IIndexStructure
{
    void Insert(Key key, RID rid);
    void Delete(Key key);
    RID? Search(Key key);
    IEnumerable<RID> RangeScan(Key fromKey, Key toKey);
}

public interface IReplacementPolicy
{
    void RecordAccess(int frameId);
    int? EvictFrame();
    void Pin(int frameId);
    void Unpin(int frameId);
}

public interface ISnapshotProvider
{
    SnapshotId CreateSnapshot(TransactionId txId);
    void ReleaseSnapshot(SnapshotId id);
    bool IsVisible(TransactionId txId, SnapshotId snapshotId);
}

public interface IAccessMethod
{
    void Open(ScanContext ctx);
    Tuple? Next();
    void Close();
}

public interface IMetadataCatalog
{
    TableMeta? GetTableMeta(TableName name);
    IndexMeta? GetIndexMeta(IndexId id);
    void UpdateMeta(CatalogObject obj);
    void DeleteMeta(ObjectId id);
}

public interface IAuthorizationManager
{
    void CheckPermission(UserId user, ObjectId obj, Action action);
    void GrantRole(UserId user, RoleId role);
    void RevokeRole(UserId user, RoleId role);
}
