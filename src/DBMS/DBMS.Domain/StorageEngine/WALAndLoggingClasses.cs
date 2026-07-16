namespace DBMS.Domain.StorageEngine;

using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class WALManager : IWALManager
{
    private LogBuffer buffer;
    private ILogWriter writer;
    private LSNGenerator lsnGen;
    
    private LSN WriteToBuffer(LogRecord record) => throw new System.NotImplementedException();
    private bool ShouldForceFlush() => throw new System.NotImplementedException();
    
    public LSN Append(LogRecord record) => throw new System.NotImplementedException();
    public void Flush(LSN upToLSN) => throw new System.NotImplementedException();
    public void TruncateBefore(LSN lsn) => throw new System.NotImplementedException();
}

public class LogWriter : ILogWriter
{
    private AbsolutePath logFilePath;
    private long currentOffset;
    
    public void Write(byte[] bytes) => throw new System.NotImplementedException();
    public void Fsync() => throw new System.NotImplementedException();
    public long CurrentOffset() => throw new System.NotImplementedException();
}

public class LSNGenerator
{
    private LSN currentLSN;
    public LSN GetNextLSN() => throw new System.NotImplementedException();
    public LSN GetCurrentLSN() => throw new System.NotImplementedException();
}
