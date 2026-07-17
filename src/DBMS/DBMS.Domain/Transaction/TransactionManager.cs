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
    
    public TransactionId Begin()
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void Commit(TransactionId txId)
    {
        throw new System.NotImplementedException();
    }
    public void Abort(TransactionId txId)
    {
        throw new System.NotImplementedException();
    }
}
