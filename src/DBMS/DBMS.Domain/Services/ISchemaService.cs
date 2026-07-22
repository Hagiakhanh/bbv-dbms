using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Builder;

namespace DBMS.Domain.Services;

public interface ISchemaService
{
    Table CreateTable(Schema schema, TableDefinition definition);
}
