namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class PageNotFoundException : StorageException
{
    public PageId MissingPage { get; }
    public PageNotFoundException(PageId missingPage) : base($"Page not found: {missingPage}")
    {
        MissingPage = missingPage;
    }
}
