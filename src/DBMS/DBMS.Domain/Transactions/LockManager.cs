using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class LockManager
{
    private Map<string, LockQueue> lockTable;
    private DeadlockDetector detector;
    public void AcquireLock(txId : int, resId : string, mode : LockMode) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void ReleaseLock(txId : int, resId : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void ReleaseAll(txId : int) 
    { 
        throw new NotImplementedException(); 
    }
}
