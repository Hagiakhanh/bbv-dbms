using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class TransactionManager
{
    private TransactionTable txTable;
    private LockManager lockMgr;
    private WALManager walMgr;
    private MVCCManager mvccMgr;
    public Transaction Begin() 
    { 
        throw new NotImplementedException(); 
    }

    public void Commit(txId : int) 
    { 
        throw new NotImplementedException(); 
    }

    public void Abort(txId : int) 
    { 
        throw new NotImplementedException(); 
    }
}
