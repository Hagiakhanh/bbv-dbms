using DBMS.API.DTOs.Indexes;

namespace DBMS.API.Services.Indexes
{
    public interface IIndexService
    {
        Task<IndexDto> CreateIndexAsync(string databaseName, string schemaName, string tableName, CreateIndexRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<IndexDto>> GetIndexesByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<IndexDto?> GetIndexByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> EnableIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> DisableIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> RebuildIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> DropIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
    }
}
