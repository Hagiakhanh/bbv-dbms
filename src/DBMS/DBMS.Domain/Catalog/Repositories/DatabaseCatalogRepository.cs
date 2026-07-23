using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Repositories;

public class DatabaseCatalogRepository : IDatabaseCatalogRepository
{
    public void Add(Database database)
    {
        throw new NotImplementedException();
    }

    public void Update(Database database)
    {
        throw new NotImplementedException();
    }

    public void Remove(int databaseId)
    {
        throw new NotImplementedException();
    }

    public Database FindById(int databaseId)
    {
        throw new NotImplementedException();
    }

    public Database FindByName(string name)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Database> GetAll()
    {
        throw new NotImplementedException();
    }

    public bool Exists(string name)
    {
        throw new NotImplementedException();
    }
}
