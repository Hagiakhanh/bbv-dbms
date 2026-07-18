using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class MVCCManager
{
    public void CreateVersion(RID rid, int txId, RecordData data)
    {
        throw new NotImplementedException();
    }

    public Row ReadVersion(RID rid, long snapshotId)
    {
        throw new NotImplementedException();
    }

    public void GarbageCollect(long olderThan)
    {
        throw new NotImplementedException();
    }
}
