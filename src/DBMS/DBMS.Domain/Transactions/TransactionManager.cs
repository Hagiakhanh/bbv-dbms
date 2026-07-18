using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class TransactionManager
{
    private object txTable;
    private LockManager lockMgr;
    private WALManager walMgr;
    private MVCCManager mvccMgr;

    public Transaction Begin()
    {
        throw new NotImplementedException();
    }

    public void Commit(int txId)
    {
        throw new NotImplementedException();
    }

    public void Abort(int txId)
    {
        throw new NotImplementedException();
    }
}
