using System;

namespace DBMS.Domain.Storage.Proxies;

public class BufferPool : BufferPoolProxy
{
    public BufferPool() : base()
    {
    }

    public BufferPool(IPageStore realStore) : base(realStore)
    {
    }

    public BufferPool(IFileManager fileManager) : base(fileManager)
    {
    }
}
