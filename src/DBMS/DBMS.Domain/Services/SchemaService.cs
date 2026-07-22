using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Builder;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Services;

public class SchemaService : ISchemaService
{
    private readonly CatalogManager _catalog;
    private readonly StorageEngine _storage;
    private readonly TableDirector _tableDirector;

    public SchemaService(CatalogManager catalog, StorageEngine storage, TableDirector tableDirector)
    {
        _catalog = catalog;
        _storage = storage;
        _tableDirector = tableDirector;
    }

    public Table CreateTable(Schema schema, TableDefinition definition)
    {
        throw new NotImplementedException();
    }
}
