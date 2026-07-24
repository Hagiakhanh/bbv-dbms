using System;
using DBMS.Domain.Server.States;
using DBMS.Domain.Security;

namespace DBMS.Domain.Server;

public class DatabaseServer
{
    public int ServerId { get; set; }
    public string Version { get; set; }
    public ServerStatus Status => _currentState.Status;
    
    private IServerState _currentState;
    
    private readonly IDbEngineFacade _engineFacade;
    private readonly IDatabaseManager _databaseManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly ISecurityManager _securityManager;
    private readonly IMonitoringManager _monitoringManager;

    public DatabaseServer(
        IDbEngineFacade engineFacade,
        IDatabaseManager databaseManager,
        IConfigurationManager configurationManager,
        ISecurityManager securityManager,
        IMonitoringManager monitoringManager)
    {
        _engineFacade = engineFacade ?? throw new ArgumentNullException(nameof(engineFacade));
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
        _securityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
        _monitoringManager = monitoringManager ?? throw new ArgumentNullException(nameof(monitoringManager));
        
        _currentState = new StoppedState();
    }

    public void SetState(IServerState state)
    {
        // _currentState = state;
        throw new NotImplementedException();
    }

    public void Start(bool safeMode)
    {
        // _currentState.Start(this, safeMode);
        throw new NotImplementedException();
    }

    public void Stop(bool force)
    {
        // _currentState.Stop(this, force);
        throw new NotImplementedException();
    }

    public void Restart()
    {
        // _currentState.Restart(this);
        throw new NotImplementedException();
    }
    
    public void Recover()
    {
        // _currentState.Recover(this);
        throw new NotImplementedException();
    }

    public void HandleSignal(string signal)
    {
        // throw new NotImplementedException();
        throw new NotImplementedException();
    }

    public ServerStatus GetStatus() => Status;

    internal void InitializeComponents(bool safeMode)
    {
        // _configurationManager.LoadConfiguration("default.config");
        // _engineFacade.Start(safeMode);
        // _monitoringManager.StartCollection();
        throw new NotImplementedException();
    }

    internal void ShutdownComponents(bool force)
    {
        // _engineFacade.Stop(force);
        throw new NotImplementedException();
    }

    internal void RestartComponents()
    {
        // _engineFacade.Restart();
        throw new NotImplementedException();
    }

    internal void RecoverComponents()
    {
        // _engineFacade.Recover();
        throw new NotImplementedException();
    }
}
