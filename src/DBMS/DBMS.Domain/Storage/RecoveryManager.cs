using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class RecoveryManager
{
    private WALManager walMgr;
    public void Recover(checkpointLSN : long) 
    {
        throw new NotImplementedException(); 
    }
}
