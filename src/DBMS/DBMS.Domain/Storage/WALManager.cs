using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class WALManager : IWALManager
{
    private object buffer;

    public long Append(object record)
    {
        throw new NotImplementedException();
    }

    public long WriteLog(object record)
    {
        throw new NotImplementedException();
    }

    public void Flush(long upToLSN)
    {
        throw new NotImplementedException();
    }

    public void Truncate(long beforeLSN)
    {
        throw new NotImplementedException();
    }

    public void Checkpoint()
    {
        throw new NotImplementedException();
    }

    public void ReplayWAL()
    {
        throw new NotImplementedException();
    }
}
