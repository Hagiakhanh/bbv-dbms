using DBMS.API.DTOs.Tables;

namespace DBMS.API.Services.Tables
{
    public interface ITableService
    {
        Task<TableDto> CreateTableAsync(string databaseName, string schemaName, CreateTableRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<TableDto>> GetTablesBySchemaAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default);
        Task<TableDto?> GetTableByNameAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<TableDto> UpdateTableAsync(string databaseName, string schemaName, string tableName, UpdateTableRequest request, CancellationToken cancellationToken = default);
        Task<bool> DropTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
    }
}
