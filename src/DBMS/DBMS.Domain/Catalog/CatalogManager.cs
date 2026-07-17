using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog;

public class CatalogManager
{
    private Map<string, object> sysTables;
    public void RegisterTable(table : Table) 
    {
        throw new NotImplementedException(); 
    }

    public Table GetTable(name : string) 
    {
        throw new NotImplementedException();
    }

    public Index GetIndex(name : string) 
    {
        throw new NotImplementedException();
    }
    
    public void DeleteMeta(id : int) 
    {
        throw new NotImplementedException();
    }
}
