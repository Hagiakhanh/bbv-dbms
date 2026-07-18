using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server;

public class DatabaseManager
{
    private CatalogManager catalog;

    public void CreateDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public void DropDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public Database GetDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public List<Database> ListDatabases()
    {
        throw new NotImplementedException();
    }
}
