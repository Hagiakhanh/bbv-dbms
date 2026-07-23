using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Repositories;

public interface IDatabaseCatalogRepository
{
    void Add(Database database);
    void Update(Database database);
    void Remove(int databaseId);
    Database FindById(int databaseId);
    Database FindByName(string name);
    IReadOnlyCollection<Database> GetAll();
    bool Exists(string name);
}
