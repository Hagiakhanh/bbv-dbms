using System;
using System.Collections.Generic;

namespace DBMS.Domain.Transactions;

public class Transaction
{
    public int TransactionId { get; set; }
    public string Status { get; set; }

    public void Begin()
    {
        throw new NotImplementedException();
    }

    public void Commit()
    {
        throw new NotImplementedException();
    }

    public void Rollback()
    {
        throw new NotImplementedException();
    }
}
