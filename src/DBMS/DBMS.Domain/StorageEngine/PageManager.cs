namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class PageManager
{
    private DiskManager diskManager;
    private SpaceManager spaceMgr;
    public PageId AllocatePage(int tableId)
    {
        throw new System.NotImplementedException();
    }
    public RawPage FetchPage(PageId id)
    {
        throw new System.NotImplementedException();
    }
    public void FreePage(PageId id)
    {
        throw new System.NotImplementedException();
    }
}
