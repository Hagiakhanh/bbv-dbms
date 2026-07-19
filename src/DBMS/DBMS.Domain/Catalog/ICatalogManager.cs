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
}
