namespace DBMS.Domain.Storage;

public interface IBufferPool : IPageStore
{
    void UnpinPage(int pageId);
    void MarkDirty(int pageId);
    Page EvictPage();
    void FlushAll();
    void FlushDirtyBuffers(string dbName);
    void FlushDirtyPagesBeforeShutdown();
}
