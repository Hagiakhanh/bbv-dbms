namespace DBMS.Domain.Transaction;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class TransactionTable
{
    private Dictionary<TransactionId, TxState> activeTxns;
    
    public TransactionTable()
    {
        this.activeTxns = new Dictionary<TransactionId, TxState>();
    }
    
    public void AddTx(TransactionId txId)
    
    {
        throw new System.NotImplementedException();
    }
    public void RemoveTx(TransactionId txId)
    {
        throw new System.NotImplementedException();
    }
    public TxState GetState(TransactionId txId)
    {
        throw new System.NotImplementedException();
    }
    public List<TransactionId> GetActiveIds()
    {
        throw new System.NotImplementedException();
    }
}
