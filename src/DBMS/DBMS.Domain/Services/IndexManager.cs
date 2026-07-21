using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Services;

public class IndexManager
{
    private readonly Dictionary<string, DBMS.Domain.Core.Index> _indexes = new();

    public void CreateIndex(string name)
    {
        throw new NotImplementedException();
    }

    public DBMS.Domain.Core.Index FindBestIndex(DBMS.Domain.Query.Query query)
    {
        throw new NotImplementedException();
    }
}
