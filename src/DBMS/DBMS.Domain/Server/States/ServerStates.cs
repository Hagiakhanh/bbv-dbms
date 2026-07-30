using System;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Server.States;

public interface IServerState
{
    ServerStatus Status { get; }
    void Start(DatabaseServer server, bool safeMode);
    void Stop(DatabaseServer server, bool force);
    void Restart(DatabaseServer server);
    void Recover(DatabaseServer server);
}

public class StoppedState : IServerState
{
    public ServerStatus Status => ServerStatus.Stopped;

    public void Start(DatabaseServer server, bool safeMode)
    {
        server.InitializeComponents(safeMode);
        server.SetState(new RunningState());
    }

    public void Stop(DatabaseServer server, bool force)
    {
        // Already stopped, do nothing
    }

    public void Restart(DatabaseServer server)
    {
        server.InitializeComponents(false);
        server.SetState(new RunningState());
    }

    public void Recover(DatabaseServer server)
    {
        server.SetState(new RecoveringState());
        server.RecoverComponents();
        server.SetState(new StoppedState());
    }
}

public class RunningState : IServerState
{
    public ServerStatus Status => ServerStatus.Running;

    public void Start(DatabaseServer server, bool safeMode)
    {
        throw new InvalidOperationException("Server is already running.");
    }

    public void Stop(DatabaseServer server, bool force)
    {
        server.ShutdownComponents(force);
        server.SetState(new StoppedState());
    }

    public void Restart(DatabaseServer server)
    {
        server.RestartComponents();
    }

    public void Recover(DatabaseServer server)
    {
        throw new InvalidOperationException("Cannot recover while running.");
    }
}

public class RecoveringState : IServerState
{
    public ServerStatus Status => ServerStatus.Recovering;

    public void Start(DatabaseServer server, bool safeMode)
    {
        throw new InvalidOperationException("Server is currently recovering.");
    }

    public void Stop(DatabaseServer server, bool force)
    {
        throw new InvalidOperationException("Cannot stop during recovery.");
    }

    public void Restart(DatabaseServer server)
    {
        throw new InvalidOperationException("Cannot restart during recovery.");
    }

    public void Recover(DatabaseServer server)
    {
        throw new InvalidOperationException("Already recovering.");
    }
}

public class FailedState : IServerState
{
    public ServerStatus Status => ServerStatus.Failed;

    public void Start(DatabaseServer server, bool safeMode)
    {
        throw new InvalidOperationException("Server is in a failed state. Recover it first.");
    }

    public void Stop(DatabaseServer server, bool force)
    {
        server.ShutdownComponents(true);
        server.SetState(new StoppedState());
    }

    public void Restart(DatabaseServer server)
    {
        throw new InvalidOperationException("Server is in a failed state. Recover it first.");
    }

    public void Recover(DatabaseServer server)
    {
        server.SetState(new RecoveringState());
        server.RecoverComponents();
        server.SetState(new StoppedState());
    }
}
