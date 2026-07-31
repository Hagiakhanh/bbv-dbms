using DBMS.API.DTOs.Schemas;

namespace DBMS.API.Services.Schemas
{
    public interface ISchemaService
    {
        Task<SchemaDto> CreateSchemaAsync(string databaseName, CreateSchemaRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<SchemaDto>> GetSchemasByDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);
        Task<SchemaDto?> GetSchemaByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<SchemaDto> RenameSchemaAsync(string name, RenameSchemaRequest request, CancellationToken cancellationToken = default);
        Task<bool> DropSchemaAsync(string name, CancellationToken cancellationToken = default);
    }
}
