using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Repositories;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Catalog;

public class CatalogManager : ICatalogManager
{
    private readonly IDatabaseCatalogRepository _databaseRepository;
    private readonly ISchemaCatalogRepository _schemaRepository;
    private readonly ITableCatalogRepository _tableRepository;

    public CatalogManager(
        IDatabaseCatalogRepository databaseRepository,
        ISchemaCatalogRepository schemaRepository,
        ITableCatalogRepository tableRepository)
    {
        _databaseRepository = databaseRepository;
        _schemaRepository = schemaRepository;
        _tableRepository = tableRepository;
    }

    // Legacy methods
    public void RegisterDatabase(string name) { throw new NotImplementedException(); }
    public void RemoveDatabase(string name) { throw new NotImplementedException(); }
    public Composite.Database GetDatabase(string name) { throw new NotImplementedException(); }
    public IEnumerable<Composite.Database> ListDatabases() { throw new NotImplementedException(); }
    public bool CheckExists(string name) { throw new NotImplementedException(); }
    public DatabaseState GetDatabaseState(string name) { throw new NotImplementedException(); }
    public bool HasSchemas(string name) { throw new NotImplementedException(); }
    public void LoadCatalog(string name) { throw new NotImplementedException(); }
    public void UpdateDatabaseName(string oldName, string newName) { throw new NotImplementedException(); }
    public void UpdateState(string name, DatabaseState state) { throw new NotImplementedException(); }
    public void RegisterExistingDatabaseFiles(string name, string filePath) { throw new NotImplementedException(); }
    public void Unregister(string name) { throw new NotImplementedException(); }

    public Index GetIndex(string name) { throw new NotImplementedException(); }
    public void DeleteMeta(int id) { throw new NotImplementedException(); }
    public Table GetTable(string name) { throw new NotImplementedException(); }

    // Repository Pattern methods
    public void RegisterDatabase(Composite.Database database) { throw new NotImplementedException(); }
    public void RegisterSchema(Schema schema) { throw new NotImplementedException(); }
    public void RegisterTable(Table table) { throw new NotImplementedException(); }
    public void UpdateDatabase(Composite.Database database) { throw new NotImplementedException(); }
    public void UpdateSchema(Schema schema) { throw new NotImplementedException(); }
    public void UpdateTable(Table table) { throw new NotImplementedException(); }
    public void RemoveDatabase(int databaseId) { throw new NotImplementedException(); }
    public void RemoveSchema(int schemaId) { throw new NotImplementedException(); }
    public void RemoveTable(int tableId) { throw new NotImplementedException(); }
    public Schema GetSchema(int databaseId, string name) { throw new NotImplementedException(); }
    public Table GetTable(int schemaId, string name) { throw new NotImplementedException(); }
    public bool ObjectExists(int parentId, string name) { throw new NotImplementedException(); }
}

