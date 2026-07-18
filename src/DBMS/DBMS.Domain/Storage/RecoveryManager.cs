using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class RecoveryManager
{
    private WALManager walMgr;

    public void Recover(long checkpointLSN)
    {
        throw new NotImplementedException();
    }
}
