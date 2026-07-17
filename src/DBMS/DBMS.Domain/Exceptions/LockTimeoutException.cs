namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

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
