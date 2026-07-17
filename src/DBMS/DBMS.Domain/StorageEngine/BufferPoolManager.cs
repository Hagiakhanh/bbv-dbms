namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class BufferPoolManager : IBufferPool
{
    private object[] frames;
    private object pageTable;
    private IReplacementPolicy policy;
    private DiskManager diskManager;
    
    private object FindInCache(PageId id)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    private object LoadFromDisk(PageId id)
    {
        throw new System.NotImplementedException();
    }
    private void EvictIfFull()
    {
        throw new System.NotImplementedException();
    }
    public object FetchPage(PageId id)
    {
        throw new System.NotImplementedException();
    }
    public void UnpinPage(PageId id)
    {
        throw new System.NotImplementedException();
    }
    public void FlushPage(PageId id)
    {
        throw new System.NotImplementedException();
    }
    public void MarkDirty(PageId id)
    {
        throw new System.NotImplementedException();
    }
}
