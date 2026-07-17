using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server;

public class DatabaseManager
{
    private CatalogManager catalog;
    public void CreateDatabase(name : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void DropDatabase(name : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    public Database GetDatabase(name : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    public List<Database> ListDatabases() 
    { 
        throw new NotImplementedException(); 
    }
}
