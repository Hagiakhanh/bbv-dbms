namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface ILogWriter
{
    void Write(byte[] bytes);
    void Fsync();
    long CurrentOffset();
}
