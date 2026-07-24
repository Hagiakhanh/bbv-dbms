using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Catalog;

public class CatalogManager : ICatalogManager
{
    public CatalogManager()
    {
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

}

