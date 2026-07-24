using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server;

public interface IMonitoringManager
{
    void Monitor();
    ServerMetrics GetMetrics();
    void StartCollection();
}

public class MonitoringManager : IMonitoringManager
{
    public void Monitor()
    {
        throw new NotImplementedException();
    }

    public ServerMetrics GetMetrics()
    {
        throw new NotImplementedException();
    }

    public void StartCollection()
    {
        throw new NotImplementedException();
    }
}
