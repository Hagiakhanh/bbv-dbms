using DBMS.API.DTOs.Tables;
using DBMS.API.Repositories.Schemas;
using DBMS.API.Repositories.Tables;
using DBMS.Domain.DatabaseObjects.Tables;

namespace DBMS.API.Services.Tables
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly ISchemaRepository _schemaRepository;

        public TableService(ITableRepository tableRepository, ISchemaRepository schemaRepository)
        {
            _tableRepository = tableRepository;
            _schemaRepository = schemaRepository;
        }

        public async Task<TableDto> CreateTableAsync(string databaseName, string schemaName, CreateTableRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Table name is required.", nameof(request.Name));
            }

            var db = string.IsNullOrWhiteSpace(databaseName) ? request.DatabaseName : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? request.SchemaName : schemaName;

            var table = new Table(request.Name);
            var createdTable = await _tableRepository.CreateAsync(db, schema, table, cancellationToken);

            return MapToDto(createdTable, db, schema);
        }

        public async Task<IEnumerable<TableDto>> GetTablesBySchemaAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            var tables = await _tableRepository.GetBySchemaAsync(db, schema, cancellationToken);
            return tables.Select(t => MapToDto(t, db, schema));
        }

        public async Task<TableDto?> GetTableByNameAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            var table = await _tableRepository.GetByNameAsync(db, schema, tableName, cancellationToken);
            return table == null ? null : MapToDto(table, db, schema);
        }

        public async Task<TableDto> UpdateTableAsync(string databaseName, string schemaName, string tableName, UpdateTableRequest request, CancellationToken cancellationToken = default)
        {
            var updated = await _tableRepository.UpdateAsync(databaseName, schemaName, tableName, request.NewName, cancellationToken);
            return MapToDto(updated, databaseName, schemaName);
        }

        public async Task<bool> DropTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            return await _tableRepository.DropAsync(databaseName, schemaName, tableName, cancellationToken);
        }

        private static TableDto MapToDto(Table table, string db, string schema)
        {
            return new TableDto
            {
                TableId = table.TableId,
                Name = table.Name,
                SchemaName = schema,
                DatabaseName = db,
                ColumnCount = table.Columns?.Count ?? 0,
                ConstraintCount = table.Constraints?.Count ?? 0,
                IndexCount = table.Indexes?.Count ?? 0
            };
        }
    }
}
