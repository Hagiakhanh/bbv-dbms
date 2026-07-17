namespace DBMS.Domain.Models;
using System;

public class LSN
{
    public long Value { get; }

    public LSN(long value)

    {

        Value = value;

    }
    public bool IsAfter(LSN other)

    {

        throw new NotImplementedException();

    }
    public LSN Next()
    {
        throw new NotImplementedException();
    }
}
