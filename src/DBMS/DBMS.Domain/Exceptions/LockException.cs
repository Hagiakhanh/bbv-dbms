namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class LockException : DbmsException
{
    public LockException(string message) : base(message) { }
}
