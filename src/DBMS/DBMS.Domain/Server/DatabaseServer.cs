using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server;

public class DatabaseServer
{
    public int ServerId { get; set; }
    public string Version { get; set; }
    public ServerStatus Status { get; set; }
    public void Start() 
    { 
        throw new NotImplementedException(); 
    }

    public void Stop() 
    { 
        throw new NotImplementedException(); 
    }
    
    public void Restart() 
    { 
        throw new NotImplementedException(); 
    }
}
