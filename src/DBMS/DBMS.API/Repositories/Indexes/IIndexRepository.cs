using DBMS.API.DTOs.Indexes;

namespace DBMS.API.Repositories.Indexes
{
    public interface IIndexRepository
    {
        Task<IndexDto> CreateAsync(string databaseName, string schemaName, string tableName, CreateIndexRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<IndexDto>> GetByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<IndexDto?> GetByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> SetEnabledAsync(string databaseName, string schemaName, string tableName, string name, bool enabled, CancellationToken cancellationToken = default);
        Task<bool> RebuildAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> DropAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
    }
}
