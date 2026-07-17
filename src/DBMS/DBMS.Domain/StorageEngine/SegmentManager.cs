namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class SegmentManager
{
    private Dictionary<int, SegmentInfo> segments;
    public int CreateSegment(int tableId)
    {
        throw new System.NotImplementedException();
    }
    public void GrowSegment(int id)
    {
        throw new System.NotImplementedException();
    }
    public void DropSegment(int id)
    {
        throw new System.NotImplementedException();
    }
}
