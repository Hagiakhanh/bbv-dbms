using System;

namespace DBMS.Domain.QueryProcessing.Execution;

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
