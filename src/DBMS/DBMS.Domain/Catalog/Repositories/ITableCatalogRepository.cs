using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Repositories;

public interface ITableCatalogRepository
{
    void Add(Table table);
    void Update(Table table);
    void Remove(int tableId);
    Table FindById(int tableId);
    Table FindByName(int schemaId, string name);
    IReadOnlyCollection<Table> GetBySchema(int schemaId);
    bool Exists(int schemaId, string name);
}
