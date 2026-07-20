using System;

namespace DBMS.Domain.Exceptions;

public class DatabaseOfflineException : Exception
{
    public DatabaseOfflineException(string message) : base(message)
    {
    }
}
