using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Repositories;

public interface ISchemaCatalogRepository
{
    void Add(Schema schema);
    void Update(Schema schema);
    void Remove(int schemaId);
    Schema FindById(int schemaId);
    Schema FindByName(int databaseId, string name);
    IReadOnlyCollection<Schema> GetByDatabase(int databaseId);
    bool Exists(int databaseId, string name);
}
