using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Repositories;

public class SchemaCatalogRepository : ISchemaCatalogRepository
{
    public void Add(Schema schema)
    {
        throw new NotImplementedException();
    }

    public void Update(Schema schema)
    {
        throw new NotImplementedException();
    }

    public void Remove(int schemaId)
    {
        throw new NotImplementedException();
    }

    public Schema FindById(int schemaId)
    {
        throw new NotImplementedException();
    }

    public Schema FindByName(int databaseId, string name)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Schema> GetByDatabase(int databaseId)
    {
        throw new NotImplementedException();
    }

    public bool Exists(int databaseId, string name)
    {
        throw new NotImplementedException();
    }
}
