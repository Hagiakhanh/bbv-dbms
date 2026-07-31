using DBMS.Domain.DatabaseObjects.Schemas;

namespace DBMS.API.Repositories.Schemas
{
    public interface ISchemaRepository
    {
        Task<Schema> CreateAsync(string databaseName, Schema schema, CancellationToken cancellationToken = default);
        Task<IEnumerable<Schema>> GetByDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);
        Task<Schema?> GetByNameAsync(string schemaName, CancellationToken cancellationToken = default);
        Task<Schema> RenameAsync(string schemaName, string newName, CancellationToken cancellationToken = default);
        Task<bool> DropAsync(string schemaName, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default);
    }
}
