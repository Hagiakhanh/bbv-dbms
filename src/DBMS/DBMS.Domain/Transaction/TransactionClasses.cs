namespace DBMS.Domain.Transaction;

using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.StorageEngine;

public class TransactionManager
{
    public TransactionTable TxTable { get; set; }
    public LockManager LockMgr { get; set; }
    public WALManager WalMgr { get; set; }

    public TxId Begin() => throw new System.NotImplementedException();
    public void Commit(TxId txId) => throw new System.NotImplementedException();
    public void Abort(TxId txId) => throw new System.NotImplementedException();
}

public class TransactionTable
{
    public Dictionary<string, object> ActiveTxns { get; set; }

    public void AddTx() => throw new System.NotImplementedException();
    public void RemoveTx() => throw new System.NotImplementedException();
}

public class LockManager
{
    public Dictionary<string, object> LockTable { get; set; }
    public DeadlockDetector Detector { get; set; }

    public bool AcquireLock(TxId txId, ResId resId) => throw new System.NotImplementedException();
    public void ReleaseLock(TxId txId, ResId resId) => throw new System.NotImplementedException();
}

public class MVCCManager
{
    public SnapshotManager SnapshotMgr { get; set; }

    public Record ReadVersion(RID rid, TxId txId) => throw new System.NotImplementedException();
    public void WriteVersion(RID rid, TxId txId) => throw new System.NotImplementedException();
}

public class SnapshotManager
{
    public List<object> ActiveSnapshots { get; set; }

    public void CreateSnapshot() => throw new System.NotImplementedException();
    public void IsVisible(TxId txId) => throw new System.NotImplementedException();
}

public class DeadlockDetector
{
    public WaitForGraph Graph { get; set; }

    public void BuildGraph() => throw new System.NotImplementedException();
    public void DetectCycle() => throw new System.NotImplementedException();
}
