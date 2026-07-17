namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class RecordManager
{
    private IBufferPool bufferPool;
    private RecordLayoutManager layoutMgr;
    private RIDGenerator ridGen;
    public RID InsertRecord(int tableId, byte[] data)
    {
        throw new System.NotImplementedException();
    }
    public void DeleteRecord(RID rid)
    {
        throw new System.NotImplementedException();
    }
    public void UpdateRecord(RID rid, byte[] data)
    {
        throw new System.NotImplementedException();
    }
    public Record? ReadRecord(RID rid)
    {
        throw new System.NotImplementedException();
    }
}
