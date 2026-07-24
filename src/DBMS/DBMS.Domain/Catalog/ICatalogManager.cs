using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Catalog;

public interface ICatalogManager
{
    void RegisterDatabase(string name);
    void RemoveDatabase(string name);
    Composite.Database GetDatabase(string name);
    IEnumerable<Composite.Database> ListDatabases();
    bool CheckExists(string name);
    DatabaseState GetDatabaseState(string name);
    bool HasSchemas(string name);
    void LoadCatalog(string name);
    void UpdateDatabaseName(string oldName, string newName);
    void UpdateState(string name, DatabaseState state);
    void RegisterExistingDatabaseFiles(string name, string filePath);
    void Unregister(string name);

    void RegisterSchema(string dbName, string schemaName);
    void RegisterTable(Table table);
    Table FindTable(string name);
    object ResolveObjectName(string name);
    void DropSchema(string name);
}
