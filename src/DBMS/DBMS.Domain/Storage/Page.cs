using System;
using System.Collections.Generic;
using DBMS.Domain.Core;

namespace DBMS.Domain.Storage;

public class Page
{
    public int PageId { get; set; }
    public byte[] Data { get; set; }
    public bool IsDirty { get; set; }
    public int PinCount { get; set; }
    public int FreeSpace { get; set; }

    public RID InsertRecord(byte[] record)
    {
        throw new NotImplementedException();
    }

    public void DeleteRecord(RID rid)
    {
        throw new NotImplementedException();
    }

    public void Compact()
    {
        throw new NotImplementedException();
    }
}
