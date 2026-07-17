namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class RIDGenerator
{
    private PageId currentPageId;
    private int nextSlotId;
    public RID GenerateNextRID(int tableId)
    {
        throw new System.NotImplementedException();
    }
}
