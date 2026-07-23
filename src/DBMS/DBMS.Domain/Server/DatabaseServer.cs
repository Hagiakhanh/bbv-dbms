using System;
using System.Collections.Generic;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Server;

public class DatabaseServer
{
    public int ServerId { get; set; }
    public string Version { get; set; }
    public ServerStatus Status { get; set; }
    
    private readonly IDbEngineFacade _engineFacade;

    public DatabaseServer(IDbEngineFacade engineFacade)
    {
        _engineFacade = engineFacade ?? throw new ArgumentNullException(nameof(engineFacade));
    }

    public void Start(bool safeMode)
    {
        _engineFacade.Start(safeMode);
        Status = ServerStatus.Running;
    }

    public void Stop(bool force)
    {
        _engineFacade.Stop(force);
        Status = ServerStatus.Stopped;
    }

    public void Restart()
    {
        _engineFacade.Restart();
    }
    
    public void Recover()
    {
        _engineFacade.Recover();
    }
}
