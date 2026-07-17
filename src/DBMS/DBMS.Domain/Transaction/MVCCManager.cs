namespace DBMS.Domain.Transaction;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class MVCCManager
{
    private ISnapshotProvider snapshotProvider;
    
    public MVCCManager(ISnapshotProvider snapshotProvider)
    {
        this.snapshotProvider = snapshotProvider;
    }
    
    public Record? ReadVersion(RID rid, long snapshotId)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void WriteVersion(RID rid, TransactionId txId, byte[] data)
    {
        throw new System.NotImplementedException();
    }
    public void Vacuum(LSN olderThan)
    {
        throw new System.NotImplementedException();
    }
}
