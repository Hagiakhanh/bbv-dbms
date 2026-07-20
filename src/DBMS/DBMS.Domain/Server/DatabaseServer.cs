using System;
using System.Collections.Generic;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Server;

public class DatabaseServer
{
    public int ServerId { get; set; }
    public string Version { get; set; }
    public ServerStatus Status { get; set; }
    
    private readonly IBufferPool _bufferPool;
    private readonly IWALManager _walManager;

    public DatabaseServer(IBufferPool bufferPool, IWALManager walManager)
    {
        _bufferPool = bufferPool ?? throw new ArgumentNullException(nameof(bufferPool));
        _walManager = walManager ?? throw new ArgumentNullException(nameof(walManager));
    }

    public void Start(bool safeMode)
    {
        throw new NotImplementedException();
    }

    public void Stop(bool force)
    {
        throw new NotImplementedException();
    }

    public void Restart()
    {
        throw new NotImplementedException();
    }
    
    public void Recover()
    {
        throw new NotImplementedException();
    }
}
