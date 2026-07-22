using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Services;

public class DatabaseService : IDatabaseService
{
    private readonly CatalogManager _catalog;

    public DatabaseService(CatalogManager catalog)
    {
        _catalog = catalog;
    }

    public Schema CreateSchema(Database database, string name)
    {
        throw new NotImplementedException();
    }
}
