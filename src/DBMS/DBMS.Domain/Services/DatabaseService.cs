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

    public void DropSchema(Database database, string name, bool cascade)
    {
        throw new NotImplementedException();
    }

    public void RenameSchema(Database database, string oldName, string newName)
    {
        throw new NotImplementedException();
    }
}
