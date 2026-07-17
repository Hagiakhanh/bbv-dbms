using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class MVCCManager
{
    public void CreateVersion(rid : RID, txId : int, data : RecordData) 
    { 
        throw new NotImplementedException(); 
    }

    public Row ReadVersion(rid : RID, snapshotId : long) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void GarbageCollect(olderThan : long) 
    { 
        throw new NotImplementedException(); 
    }
}
