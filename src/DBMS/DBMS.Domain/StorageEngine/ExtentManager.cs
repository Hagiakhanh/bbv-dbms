namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class ExtentManager
{
    private Dictionary<int, ExtentInfo> extents;
    private SegmentManager segMgr;
    public int AllocateExtent(int segId)
    {
        throw new System.NotImplementedException();
    }
    public void FreeExtent(int id)
    {
        throw new System.NotImplementedException();
    }
}
