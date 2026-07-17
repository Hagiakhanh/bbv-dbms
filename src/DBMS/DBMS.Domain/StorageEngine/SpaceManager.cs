namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class SpaceManager
{
    private ExtentManager extentMgr;
    public int AllocateSpace(int size)
    {
        throw new System.NotImplementedException();
    }
    public void FreeSpace(int extent)
    {
        throw new System.NotImplementedException();
    }
    public long AvailableSpace()
    {
        throw new System.NotImplementedException();
    }
}
