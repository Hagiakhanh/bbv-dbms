namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class StorageException : DbmsException
{
    public StorageException(string message) : base(message) { }
}
