using System;
using System.Collections.Generic;
using DBMS.Domain.Core;

namespace DBMS.Domain.Catalog.Services;

public class CatalogManager : ICatalogManager
{
    public CatalogManager()
    {
    }

    public void RegisterDatabase(string name) { throw new NotImplementedException(); }
    public void RemoveDatabase(string name) { throw new NotImplementedException(); }
    public Database GetDatabase(string name) { throw new NotImplementedException(); }
    public IEnumerable<Database> ListDatabases() { throw new NotImplementedException(); }
    public bool CheckExists(string name) { throw new NotImplementedException(); }
    public DatabaseState GetDatabaseState(string name) { throw new NotImplementedException(); }
    public bool HasSchemas(string name) { throw new NotImplementedException(); }
    public void LoadCatalog(string name) { throw new NotImplementedException(); }
    public void UpdateDatabaseName(string oldName, string newName) { throw new NotImplementedException(); }
    public void UpdateState(string name, DatabaseState state) { throw new NotImplementedException(); }
    public void RegisterExistingDatabaseFiles(string name, string filePath) { throw new NotImplementedException(); }
    public void Unregister(string name) { throw new NotImplementedException(); }

    public void RegisterSchema(string dbName, string schemaName) { throw new NotImplementedException(); }
    public void RegisterTable(Table table) { throw new NotImplementedException(); }
    public Table FindTable(string name) { throw new NotImplementedException(); }
    public object ResolveObjectName(string name) { throw new NotImplementedException(); }
    public void DropSchema(string name) { throw new NotImplementedException(); }

    public Index GetIndex(string name) { throw new NotImplementedException(); }
    public void DeleteMeta(int id) { throw new NotImplementedException(); }
    public Table GetTable(string name) { throw new NotImplementedException(); }
}
