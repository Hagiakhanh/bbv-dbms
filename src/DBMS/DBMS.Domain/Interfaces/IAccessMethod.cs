namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IAccessMethod
{
    void Open(ScanContext ctx);
    Tuple? Next();
    void Close();
}
