using System;
using System.Collections.Generic;

namespace DBMS.Domain.Services;

public class SchemaService
{
    private CatalogManager catalog;
    private StorageEngine storage;
    public Table CreateTable(schema : Schema, def : TableDef) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void DropTable(schema : Schema, name : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    public View CreateView(schema : Schema, name : string, query : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void DropView(schema : Schema, name : string) 
    { 
        throw new NotImplementedException(); 
    }
}
