using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server;

public enum ServerStatus
{
    Starting,
    Running,
    Stopping,
    Stopped,
    Recovering,
    Failed
}

public class ServerMetrics
{
    public double CpuUsage { get; set; }
    public long MemoryUsageBytes { get; set; }
    public int ActiveConnections { get; set; }
    public long TotalTransactions { get; set; }
}
