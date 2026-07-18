using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog;

public class CatalogManager
{
    private Dictionary<string, object> sysTables;

    public void RegisterTable(Table table)
    {
        throw new NotImplementedException();
    }

    public Table GetTable(string name)
    {
        throw new NotImplementedException();
    }

    public Core.Index GetIndex(string name)
    {
        throw new NotImplementedException();
    }

    public void DeleteMeta(int id)
    {
        throw new NotImplementedException();
    }
}
