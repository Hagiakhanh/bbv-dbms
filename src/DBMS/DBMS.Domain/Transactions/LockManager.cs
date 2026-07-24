using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class LockManager
{
    private Dictionary<string, object> lockTable;
    private object detector;

    public void AcquireLock(int txId, string resId, string mode)
    {
        throw new NotImplementedException();
    }

    public void AcquireSharedLock(int txId, string resId)
    {
        AcquireLock(txId, resId, "Shared");
    }

    public void AcquireExclusiveLock(int txId, string resId)
    {
        AcquireLock(txId, resId, "Exclusive");
    }

    public void ReleaseLock(int txId, string resId)
    {
        throw new NotImplementedException();
    }

    public void ReleaseAll(int txId)
    {
        throw new NotImplementedException();
    }

    public void DetectDeadlock()
    {
        throw new NotImplementedException();
    }
}
