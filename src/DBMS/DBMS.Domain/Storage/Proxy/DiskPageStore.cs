using System;

namespace DBMS.Domain.Storage.Proxy;

public class DiskPageStore : IPageStore
{
    private readonly IFileManager _fileManager;

    public DiskPageStore()
    {
        _fileManager = new FileManager();
    }

    public DiskPageStore(IFileManager fileManager)
    {
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
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
}
