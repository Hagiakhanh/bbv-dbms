using System;
using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Repositories;

public class TableCatalogRepository : ITableCatalogRepository
{
    public void Add(Table table)
    {
        throw new NotImplementedException();
    }

    public void Update(Table table)
    {
        throw new NotImplementedException();
    }

    public void Remove(int tableId)
    {
        throw new NotImplementedException();
    }

    public Table FindById(int tableId)
    {
        throw new NotImplementedException();
    }

    public Table FindByName(int schemaId, string name)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Table> GetBySchema(int schemaId)
    {
        throw new NotImplementedException();
    }

    public bool Exists(int schemaId, string name)
    {
        throw new NotImplementedException();
    }
}
