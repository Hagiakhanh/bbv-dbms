namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class LRUPolicy : IReplacementPolicy
{
    private int frameCount;
    private Dictionary<int, object> usageOrder;
    public void RecordAccess(int frameId)
    {
        throw new System.NotImplementedException();
    }
    public int? EvictFrame()
    {
        throw new System.NotImplementedException();
    }
    public void Pin(int frameId)
    {
        throw new System.NotImplementedException();
    }
    public void Unpin(int frameId)
    {
        throw new System.NotImplementedException();
    }
}
