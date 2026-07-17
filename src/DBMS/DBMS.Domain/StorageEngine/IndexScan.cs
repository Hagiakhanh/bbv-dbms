namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class IndexScan : IAccessMethod
{
    private int indexId;
    private Key searchKey;
    private IIndexStructure index;
    public void Open(ScanContext ctx)
    {
        throw new System.NotImplementedException();
    }
    public Tuple? Next()
    {
        throw new System.NotImplementedException();
    }
    public void Close()
    {
        throw new System.NotImplementedException();
    }
}
