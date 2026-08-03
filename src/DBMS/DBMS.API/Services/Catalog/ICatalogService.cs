using DBMS.API.DTOs.Catalog;

namespace DBMS.API.Services.Catalog
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogTreeNodeDto>> GetCatalogTreeAsync(string? databaseName = null, int? depth = null, CancellationToken cancellationToken = default);
        Task<DatabaseMetadataDto?> GetDatabaseMetadataAsync(string dbName, CancellationToken cancellationToken = default);
        Task<SchemaMetadataDto?> GetSchemaMetadataAsync(string schemaName, string? dbName = null, CancellationToken cancellationToken = default);
        Task<TableMetadataDto?> GetTableMetadataAsync(string tableName, string? schemaName = null, string? dbName = null, CancellationToken cancellationToken = default);
    }
}
