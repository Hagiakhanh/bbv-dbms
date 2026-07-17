namespace DBMS.Domain.StorageEngine;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class LogWriter : ILogWriter
{
    private string logFilePath;
    private long currentOffset;

    public LogWriter(string logFilePath)
    {
        this.logFilePath = logFilePath;
    }
    
    public void Write(byte[] bytes)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void Fsync()
    {
        throw new System.NotImplementedException();
    }
    public long CurrentOffset()
    {
        throw new System.NotImplementedException();
    }
}
