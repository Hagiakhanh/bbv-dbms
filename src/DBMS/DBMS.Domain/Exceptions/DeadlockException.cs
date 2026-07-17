namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class DeadlockException : LockException
{
    public TransactionId Victim { get; }

    public DeadlockException(TransactionId victim) : base($"Deadlock detected, victim tx: {victim}")
    {
        Victim = victim;
    }
}
