using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Factory;

public interface IDatabaseFactory
{
    Database Create(DatabaseCreationOptions options);
}
