namespace DBMS.Domain.Exceptions;

using System;
using DBMS.Domain.Models;

public class DbmsException : Exception
{
    public DbmsException(string message) : base(message) { }
    public DbmsException(string message, Exception innerException) : base(message, innerException) { }
}

public class StorageException : DbmsException
{
    public StorageException(string message) : base(message) { }
}

public class PageNotFoundException : StorageException
{
    public PageId MissingPage { get; }
    public PageNotFoundException(PageId missingPage) : base($"Page not found: {missingPage}")
    {
        MissingPage = missingPage;
    }
}

public class LockException : DbmsException
{
    public LockException(string message) : base(message) { }
}

public class LockTimeoutException : LockException
{
    public TransactionId TxId { get; }
    public long WaitedMs { get; }

    public LockTimeoutException(TransactionId txId, long waitedMs) : base($"Lock timeout for tx {txId} after {waitedMs}ms")
    {
        TxId = txId;
        WaitedMs = waitedMs;
    }
}

public class DeadlockException : LockException
{
    public TransactionId Victim { get; }

    public DeadlockException(TransactionId victim) : base($"Deadlock detected, victim tx: {victim}")
    {
        Victim = victim;
    }
}

public class SqlException : DbmsException
{
    public SqlException(string message) : base(message) { }
}

public class SqlSyntaxException : SqlException
{
    public int Line { get; }
    public int Column { get; }

    public SqlSyntaxException(int line, int column) : base($"Syntax error at line {line}, column {column}")
    {
        Line = line;
        Column = column;
    }
}

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
