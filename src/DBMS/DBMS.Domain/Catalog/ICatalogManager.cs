using System.Collections.Generic;
using DBMS.Domain.Core;

namespace DBMS.Domain.Catalog;

public interface ICatalogManager
{
    void RegisterDatabase(string name);
    void RemoveDatabase(string name);
    Database GetDatabase(string name);
    IEnumerable<Database> ListDatabases();
    bool CheckExists(string name);
    DatabaseState GetDatabaseState(string name);
    bool HasSchemas(string name);
    void LoadCatalog(string name);
    void UpdateDatabaseName(string oldName, string newName);
    void UpdateState(string name, DatabaseState state);
    void RegisterExistingDatabaseFiles(string name, string filePath);
    void Unregister(string name);

    // Repository Pattern methods
    void RegisterDatabase(Database database);
    void RegisterSchema(Schema schema);
    void RegisterTable(Table table);
    void UpdateDatabase(Database database);
    void UpdateSchema(Schema schema);
    void UpdateTable(Table table);
    void RemoveDatabase(int databaseId);
    void RemoveSchema(int schemaId);
    void RemoveTable(int tableId);
    Schema GetSchema(int databaseId, string name);
    Table GetTable(int schemaId, string name);
    bool ObjectExists(int parentId, string name);
}
