namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class DbmsException : Exception
{
    public DbmsException(string message) : base(message) { }
    public DbmsException(string message, Exception innerException) : base(message, innerException) { }
}
