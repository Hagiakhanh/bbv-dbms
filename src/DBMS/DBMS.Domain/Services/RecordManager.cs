using System;
using System.Collections.Generic;

namespace DBMS.Domain.Services;

public class RecordManager
{
    private StorageEngine storage;
    private CatalogManager catalog;
    public RID Insert(table : Table, row : Row) 
    { 
        throw new NotImplementedException(); 
    }

    public void Update(table : Table, rid : RID, row : Row) 
    { 
        throw new NotImplementedException(); 
    }

    public void Delete(table : Table, rid : RID) 
    { 
        throw new NotImplementedException(); 
    }

    public Row Read(table : Table, rid : RID) 
    { 
        throw new NotImplementedException(); 
    }
    
    public List<Row> Scan(table : Table) 
    { 
        throw new NotImplementedException(); 
    }
}
