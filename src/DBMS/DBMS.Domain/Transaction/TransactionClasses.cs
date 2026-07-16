namespace DBMS.Domain.Transaction;

using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class TransactionManager
{
    private TransactionTable txTable;
    private LockManager lockMgr;
    private IWALManager walMgr;
    private MVCCManager mvccMgr;
    
    public TransactionManager(TransactionTable txTable, LockManager lockMgr, IWALManager walMgr, MVCCManager mvccMgr)
    {
        this.txTable = txTable;
        this.lockMgr = lockMgr;
        this.walMgr = walMgr;
        this.mvccMgr = mvccMgr;
    }
    
    public TransactionId Begin() => throw new System.NotImplementedException();
    public void Commit(TransactionId txId) => throw new System.NotImplementedException();
    public void Abort(TransactionId txId) => throw new System.NotImplementedException();
}

public class TransactionTable
{
    private Dictionary<TransactionId, TxState> activeTxns;
    
    public TransactionTable()
    {
        this.activeTxns = new Dictionary<TransactionId, TxState>();
    }
    
    public void AddTx(TransactionId txId) => throw new System.NotImplementedException();
    public void RemoveTx(TransactionId txId) => throw new System.NotImplementedException();
    public TxState GetState(TransactionId txId) => throw new System.NotImplementedException();
    public List<TransactionId> GetActiveIds() => throw new System.NotImplementedException();
}

public class LockManager
{
    private Dictionary<ResourceId, LockQueue> lockTable;
    private DeadlockDetector detector;
    
    public LockManager(DeadlockDetector detector)
    {
        this.detector = detector;
        this.lockTable = new Dictionary<ResourceId, LockQueue>();
    }
    
    private void WaitFor(TransactionId txId, ResourceId resId) => throw new System.NotImplementedException();
    public void AcquireLock(TransactionId txId, ResourceId resId, LockMode mode) => throw new System.NotImplementedException();
    public void ReleaseLock(TransactionId txId, ResourceId resId) => throw new System.NotImplementedException();
    public void ReleaseAll(TransactionId txId) => throw new System.NotImplementedException();
}

public class MVCCManager
{
    private ISnapshotProvider snapshotProvider;
    
    public MVCCManager(ISnapshotProvider snapshotProvider)
    {
        this.snapshotProvider = snapshotProvider;
    }
    
    public Record? ReadVersion(RID rid, SnapshotId snapshotId) => throw new System.NotImplementedException();
    public void WriteVersion(RID rid, TransactionId txId, byte[] data) => throw new System.NotImplementedException();
    public void Vacuum(LSN olderThan) => throw new System.NotImplementedException();
}

public class SnapshotManager : ISnapshotProvider
{
    private List<Snapshot> activeSnapshots;
    
    public SnapshotManager()
    {
        this.activeSnapshots = new List<Snapshot>();
    }
    
    private Snapshot BuildSnapshot(TransactionId txId) => throw new System.NotImplementedException();
    public SnapshotId CreateSnapshot(TransactionId txId) => throw new System.NotImplementedException();
    public void ReleaseSnapshot(SnapshotId id) => throw new System.NotImplementedException();
    public bool IsVisible(TransactionId txId, SnapshotId snapshotId) => throw new System.NotImplementedException();
}

public class DeadlockDetector
{
    private WaitForGraph graph;
    
    public DeadlockDetector()
    {
        this.graph = new WaitForGraph();
    }
    
    private WaitForGraph BuildGraph(TransactionTable txTable) => throw new System.NotImplementedException();
    private List<TransactionId>? DetectCycle(WaitForGraph graph) => throw new System.NotImplementedException();
    private TransactionId SelectVictim(List<TransactionId> cycle) => throw new System.NotImplementedException();
    
    public void Check(TransactionTable txTable) => throw new System.NotImplementedException();
}
