namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IWALManager
{
    LSN Append(LogRecord record);
    void Flush(LSN upToLSN);
    void TruncateBefore(LSN lsn);
}
