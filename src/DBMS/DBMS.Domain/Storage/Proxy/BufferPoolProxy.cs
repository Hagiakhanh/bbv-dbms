using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage.Proxy;

public class BufferPoolProxy : IBufferPool, IPageStore
{
    private readonly Dictionary<int, BufferFrame> _frames = new();
    private readonly IPageStore _realStore;
    private readonly IReplacementPolicy? _replacementPolicy;

    public BufferPoolProxy()
    {
        _realStore = new DiskPageStore();
    }

    public BufferPoolProxy(IPageStore realStore)
    {
        _realStore = realStore ?? throw new ArgumentNullException(nameof(realStore));
    }

    public BufferPoolProxy(IPageStore realStore, IReplacementPolicy replacementPolicy)
    {
        _realStore = realStore ?? throw new ArgumentNullException(nameof(realStore));
        _replacementPolicy = replacementPolicy;
    }

    public BufferPoolProxy(IFileManager fileManager)
    {
        _realStore = new DiskPageStore(fileManager);
    }

    public Page FetchPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void FlushPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public int AllocatePage(int tableId)
    {
        throw new NotImplementedException();
    }

    public void UnpinPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void MarkDirty(int pageId)
    {
        throw new NotImplementedException();
    }

    public BufferFrame EvictFrame()
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
