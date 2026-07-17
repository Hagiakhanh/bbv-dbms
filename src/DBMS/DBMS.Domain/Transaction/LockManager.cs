namespace DBMS.Domain.Transaction;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class LockManager
{
    private Dictionary<string, LockQueue> lockTable;
    private DeadlockDetector detector;
    
    public LockManager(DeadlockDetector detector)
    {
        this.detector = detector;
        this.lockTable = new Dictionary<string, LockQueue>();
    }
    
    private void WaitFor(TransactionId txId, string resId)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void AcquireLock(TransactionId txId, string resId, LockMode mode)
    {
        throw new System.NotImplementedException();
    }
    public void ReleaseLock(TransactionId txId, string resId)
    {
        throw new System.NotImplementedException();
    }
    public void ReleaseAll(TransactionId txId)
    {
        throw new System.NotImplementedException();
    }
}
