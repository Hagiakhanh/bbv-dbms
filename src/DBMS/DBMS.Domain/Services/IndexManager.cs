using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Services;

public class IndexManager
{
    private readonly Dictionary<string, Index> _indexes = new();

    public void Register(Index index)
    {
        throw new NotImplementedException();
    }

    public void Drop(string name)
    {
        throw new NotImplementedException();
    }

    public Index Find(string name)
    {
        throw new NotImplementedException();
    }

    public Index FindBestIndex(Query query)
    {
        throw new NotImplementedException();
    }

    public void Rebuild(Index index)
    {
        throw new NotImplementedException();
    }
}
