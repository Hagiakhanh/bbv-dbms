
namespace DBMS.Domain.DatabaseObjects.Databases;

public interface IDatabaseFactory
{
    Database Create(DatabaseCreationOptions options);
}
