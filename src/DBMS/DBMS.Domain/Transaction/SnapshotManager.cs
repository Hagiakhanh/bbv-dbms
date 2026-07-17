namespace DBMS.Domain.Transaction;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class SnapshotManager : ISnapshotProvider
{
    private List<Snapshot> activeSnapshots;
    
    public SnapshotManager()
    {
        this.activeSnapshots = new List<Snapshot>();
    }
    
    private Snapshot BuildSnapshot(TransactionId txId)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public long CreateSnapshot(TransactionId txId)
    {
        throw new System.NotImplementedException();
    }
    public void ReleaseSnapshot(long id)
    {
        throw new System.NotImplementedException();
    }
    public bool IsVisible(TransactionId txId, long snapshotId)
    {
        throw new System.NotImplementedException();
    }
}
