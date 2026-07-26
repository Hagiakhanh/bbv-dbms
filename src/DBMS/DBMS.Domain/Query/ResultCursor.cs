using System;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Query;

public class ResultCursor
{
    public virtual bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public virtual Row Current => throw new NotImplementedException();

    public virtual void Close()
    {
        throw new NotImplementedException();
    }
}
