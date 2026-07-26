using System;

namespace DBMS.Domain.Storage;

public class BufferPool : IBufferPool
{
    private Page[] _frames;
    private string _policy;
    private readonly IFileManager _fileManager;

    public BufferPool()
    {
    }

    public BufferPool(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public Page FetchPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void UnpinPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void FlushPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void MarkDirty(int pageId)
    {
        throw new NotImplementedException();
    }

    public Page EvictPage()
    {
        throw new NotImplementedException();
    }

    public void FlushAll()
    {
        throw new NotImplementedException();
    }

    public void FlushDirtyBuffers(string dbName)
    {
        throw new NotImplementedException();
    }

    public void FlushDirtyPagesBeforeShutdown()
    {
        throw new NotImplementedException();
    }
}
