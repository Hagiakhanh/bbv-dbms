namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class PageFormatter
{
    private PageHeader FormatHeader(RawPage page)
    {
        throw new System.NotImplementedException();
    }
    private int ComputeFreeSpace(RawPage page)
    {
        throw new System.NotImplementedException();
    }
    public FormattedPage FormatSlottedPage(RawPage page)
    {
        throw new System.NotImplementedException();
    }
    public int InsertTuple(FormattedPage page, byte[] tuple)
    {
        throw new System.NotImplementedException();
    }
}
