using System;

namespace DBMS.Domain.Storage;

public class StorageEngine : IStorageEngine
{
    private readonly IBufferPool _bufferPool;
    private readonly IFileManager _fileManager;

    public StorageEngine()
    {
    }

    public StorageEngine(IBufferPool bufferPool, IFileManager fileManager)
    {
        _bufferPool = bufferPool;
        _fileManager = fileManager;
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
}
