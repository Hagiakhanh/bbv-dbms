using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Services;

public class IndexManager
{
    private readonly Dictionary<string, DBMS.Domain.Catalog.Strategy.Index> _indexes = new();

    public void Register(DBMS.Domain.Catalog.Strategy.Index index)
    {
        throw new NotImplementedException();
    }

    public void Drop(string name)
    {
        throw new NotImplementedException();
    }

    public DBMS.Domain.Catalog.Strategy.Index Find(string name)
    {
        throw new NotImplementedException();
    }

    public DBMS.Domain.Catalog.Strategy.Index FindBestIndex(DBMS.Domain.Query.Query query)
    {
        throw new NotImplementedException();
    }

    public void Rebuild(DBMS.Domain.Catalog.Strategy.Index index)
    {
        throw new NotImplementedException();
    }
}

