namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IBufferPool
{
    object FetchPage(PageId id);
    void UnpinPage(PageId id);
    void FlushPage(PageId id);
    void MarkDirty(PageId id);
}
