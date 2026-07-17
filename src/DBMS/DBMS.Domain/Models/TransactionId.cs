namespace DBMS.Domain.Models;
using System;

public class TransactionId
{
    public long Id { get; }

    public TransactionId(long id)

    {

        Id = id;

    }
    public bool IsValid()

    {

        throw new NotImplementedException();

    }
}
