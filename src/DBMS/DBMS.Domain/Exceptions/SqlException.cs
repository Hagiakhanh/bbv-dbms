namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class SqlException : DbmsException
{
    public SqlException(string message) : base(message) { }
}
