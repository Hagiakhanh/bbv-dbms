using System;
using DBMS.Domain.Catalog;

namespace DBMS.Domain.Services.Facade;

public class DatabaseService : IDatabaseService
{
    private readonly ICatalogManager _catalog;

    public DatabaseService(ICatalogManager catalog)
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
