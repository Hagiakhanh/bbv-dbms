using DBMS.API.DTOs.Schemas;
using DBMS.API.Repositories.Databases;
using DBMS.API.Repositories.Schemas;
using DBMS.Domain.DatabaseObjects.Schemas;

namespace DBMS.API.Services.Schemas
{
    public class SchemaService : ISchemaService
    {
        private readonly ISchemaRepository _schemaRepository;
        private readonly IDatabaseRepository _databaseRepository;

        public SchemaService(ISchemaRepository schemaRepository, IDatabaseRepository databaseRepository)
        {
            _schemaRepository = schemaRepository;
            _databaseRepository = databaseRepository;
        }

        public async Task<SchemaDto> CreateSchemaAsync(string databaseName, CreateSchemaRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Schema name is required.", nameof(request.Name));
            }

            var dbExists = await _databaseRepository.ExistsAsync(databaseName, cancellationToken);
            if (!dbExists)
            {
                throw new KeyNotFoundException($"Database '{databaseName}' does not exist.");
            }

            var schema = new Schema(request.Name);
            var createdSchema = await _schemaRepository.CreateAsync(databaseName, schema, cancellationToken);

            return MapToDto(createdSchema, databaseName, string.IsNullOrWhiteSpace(request.Owner) ? "dbo" : request.Owner);
        }

        public async Task<IEnumerable<SchemaDto>> GetSchemasByDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            var dbExists = await _databaseRepository.ExistsAsync(databaseName, cancellationToken);
            if (!dbExists)
            {
                throw new KeyNotFoundException($"Database '{databaseName}' does not exist.");
            }

            var schemas = await _schemaRepository.GetByDatabaseAsync(databaseName, cancellationToken);
            return schemas.Select(s => MapToDto(s, databaseName));
        }

        public async Task<SchemaDto?> GetSchemaByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var schema = await _schemaRepository.GetByNameAsync(name, cancellationToken);
            return schema == null ? null : MapToDto(schema, "master");
        }

        public async Task<SchemaDto> RenameSchemaAsync(string name, RenameSchemaRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.NewName))
            {
                throw new ArgumentException("New schema name is required.", nameof(request.NewName));
            }

            var updatedSchema = await _schemaRepository.RenameAsync(name, request.NewName, cancellationToken);
            return MapToDto(updatedSchema, "master");
        }

        public async Task<bool> DropSchemaAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _schemaRepository.DropAsync(name, cancellationToken);
        }

        private static SchemaDto MapToDto(Schema schema, string dbName, string owner = "dbo")
        {
            return new SchemaDto
            {
                SchemaId = schema.SchemaId,
                Name = schema.Name,
                Owner = owner,
                DatabaseName = dbName,
                TableCount = schema.Tables?.Count ?? 0
            };
        }
    }
}
