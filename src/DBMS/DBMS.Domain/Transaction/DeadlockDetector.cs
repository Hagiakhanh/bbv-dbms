namespace DBMS.Domain.Transaction;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class DeadlockDetector
{
    private WaitForGraph graph;
    
    public DeadlockDetector()
    {
        this.graph = new WaitForGraph();
    }
    
    private WaitForGraph BuildGraph(TransactionTable txTable)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    private List<TransactionId>? DetectCycle(WaitForGraph graph)
    {
        throw new System.NotImplementedException();
    }
    private TransactionId SelectVictim(List<TransactionId> cycle)
    {
        throw new System.NotImplementedException();
    }
    public void Check(TransactionTable txTable)
    {
        throw new System.NotImplementedException();
    }
}
