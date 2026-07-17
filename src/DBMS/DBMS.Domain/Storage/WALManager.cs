using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class WALManager
{
    private LogBuffer buffer;
    public long Append(record : LogRecord) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void Flush(upToLSN : long) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void Truncate(beforeLSN : long) 
    { 
        throw new NotImplementedException(); 
    }
}
