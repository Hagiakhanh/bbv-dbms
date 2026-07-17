namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class TableScan : IAccessMethod
{
    private int tableId;
    private RID currentRid;
    private RecordManager recordMgr;
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
