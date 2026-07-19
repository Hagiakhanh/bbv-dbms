using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog;
using DBMS.Domain.Core;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Server;

public class DatabaseManager
{
    private readonly ICatalogManager _catalog;
    private readonly IConnectionPool _connectionPool;

    public DatabaseManager(ICatalogManager catalog, IConnectionPool connectionPool)
    {
        if (catalog == null)
        {
            throw new ArgumentNullException(nameof(catalog));
        }

        if (connectionPool == null)
        {
            throw new ArgumentNullException(nameof(connectionPool));
        }

        _catalog = catalog;
        _connectionPool = connectionPool;
    }

    public void CreateDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public void DropDatabase(string name, bool cascade = false)
    {
        throw new NotImplementedException();
    }

    public Database GetDatabase(string name)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Database> ListDatabases()
    {
        throw new NotImplementedException();
    }
}
