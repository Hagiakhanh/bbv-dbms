using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Services;

public interface IDatabaseService
{
    Schema CreateSchema(Database database, string name);
}
