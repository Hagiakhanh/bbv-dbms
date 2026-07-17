namespace DBMS.Domain.Models;
using System;

public class PageId : IEquatable<PageId>
{
    public int FileId { get; }
    public int PageNumber { get; }

    public PageId(int fileId, int pageNumber)
    {
        FileId = fileId;
        PageNumber = pageNumber;
    }

    public bool Equals(PageId? other)

    {

        throw new NotImplementedException();

    }
    public override string ToString()
    {
        throw new NotImplementedException();
    }
}
