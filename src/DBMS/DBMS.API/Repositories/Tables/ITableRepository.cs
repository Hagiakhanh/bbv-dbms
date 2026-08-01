using DBMS.Domain.DatabaseObjects.Tables;

namespace DBMS.API.Repositories.Tables
{
    public interface ITableRepository
    {
        Task<Table> CreateAsync(string databaseName, string schemaName, Table table, CancellationToken cancellationToken = default);
        Task<IEnumerable<Table>> GetBySchemaAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default);
        Task<Table?> GetByNameAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<Table> UpdateAsync(string databaseName, string schemaName, string tableName, string? newName, CancellationToken cancellationToken = default);
        Task<bool> DropAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
    }
}
