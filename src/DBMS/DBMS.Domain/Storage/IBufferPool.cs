namespace DBMS.Domain.Storage;

public interface IBufferPool
{
    void FlushDirtyBuffers(string dbName);
    void FlushDirtyPagesBeforeShutdown();
}
