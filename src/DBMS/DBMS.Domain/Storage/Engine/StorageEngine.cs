using System;

namespace DBMS.Domain.Storage.Engine;

public class StorageEngine : IStorageEngine
{
    private readonly IPageStore _pageStore;
    private readonly IWALManager _walManager;

    public StorageEngine()
    {
        _pageStore = new BufferPoolProxy();
        _walManager = new WALManager();
    }

    public StorageEngine(IPageStore pageStore, IWALManager walManager)
    {
        _pageStore = pageStore ?? throw new ArgumentNullException(nameof(pageStore));
        _walManager = walManager ?? throw new ArgumentNullException(nameof(walManager));
    }

    public StorageEngine(IBufferPool bufferPool, IFileManager fileManager)
    {
        _pageStore = bufferPool ?? new BufferPoolProxy(fileManager);
        _walManager = new WALManager();
    }

    public void InitializeStorageEngine(string dbName)
    {
        throw new NotImplementedException();
    }

    public void AllocateDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public void DeallocateDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public byte[] ReadPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void WritePage(int pageId, byte[] data)
    {
        throw new NotImplementedException();
    }

    public int AllocatePage(int tableId)
    {
        throw new NotImplementedException();
    }

    public void FlushPage(int pageId)
    {
        throw new NotImplementedException();
    }
}
