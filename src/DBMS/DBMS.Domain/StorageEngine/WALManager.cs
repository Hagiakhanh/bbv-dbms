namespace DBMS.Domain.StorageEngine;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class WALManager : IWALManager
{
    private LogBuffer buffer;
    private ILogWriter writer;
    private LSNGenerator lsnGen;

    public WALManager(ILogWriter writer, LSNGenerator lsnGen)
    {
        this.writer = writer;
        this.lsnGen = lsnGen;
        this.buffer = new LogBuffer();
    }
    
    private LSN WriteToBuffer(LogRecord record)
    {
        throw new System.NotImplementedException();
    }
    private bool ShouldForceFlush()
    {
        throw new System.NotImplementedException();
    }
    public LSN Append(LogRecord record)
    {
        throw new System.NotImplementedException();
    }
    public void Flush(LSN upToLSN)
    {
        throw new System.NotImplementedException();
    }
    public void TruncateBefore(LSN lsn)
    {
        throw new System.NotImplementedException();
    }
}
