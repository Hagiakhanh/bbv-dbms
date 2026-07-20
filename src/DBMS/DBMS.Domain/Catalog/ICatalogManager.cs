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
    void LoadCatalog(string name);
    void UpdateDatabaseName(string oldName, string newName);
    void UpdateState(string name, DatabaseState state);
    void RegisterExistingDatabaseFiles(string name, string filePath);
    void Unregister(string name);
}
