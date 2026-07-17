namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IReplacementPolicy
{
    void RecordAccess(int frameId);
    int? EvictFrame();
    void Pin(int frameId);
    void Unpin(int frameId);
}
