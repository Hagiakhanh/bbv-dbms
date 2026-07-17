namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class PermissionDeniedException : SqlException
{
    public string User { get; }
    public string Object { get; }

    public PermissionDeniedException(string user, string obj) : base($"Permission denied for user {user} on {obj}")
    {
        User = user;
        Object = obj;
    }
}
