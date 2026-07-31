using DBMS.API.DTOs.Columns;
using DBMS.API.Repositories.Columns;
using DBMS.Domain.Core;
using DBMS.Domain.DatabaseObjects.Columns;

namespace DBMS.API.Services.Columns
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnRepository _columnRepository;

        public ColumnService(IColumnRepository columnRepository)
        {
            _columnRepository = columnRepository;
        }

        public async Task<ColumnDto> CreateColumnAsync(CreateColumnRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Column name is required.", nameof(request.Name));
            }

            var targetTable = string.IsNullOrWhiteSpace(request.TableName) ? "Users" : request.TableName;
            var dataType = ParseDataType(request.DataType);

            var column = new Column
            {
                Name = request.Name,
                DataType = dataType,
                Nullable = request.IsNullable,
                DefaultValue = request.DefaultValue ?? string.Empty
            };

            var created = await _columnRepository.CreateAsync(targetTable, column, cancellationToken);
            return MapToDto(created, targetTable);
        }

        public async Task<IEnumerable<ColumnDto>> GetColumnsByTableAsync(string tableName, CancellationToken cancellationToken = default)
        {
            var cols = await _columnRepository.GetByTableAsync(tableName, cancellationToken);
            return cols.Select(c => MapToDto(c, tableName));
        }

        public async Task<ColumnDto?> GetColumnByNameAsync(string tableName, string columnName, CancellationToken cancellationToken = default)
        {
            var col = await _columnRepository.GetByNameAsync(tableName, columnName);
            return col == null ? null : MapToDto(col, tableName);
        }

        public async Task<ColumnDto> UpdateColumnAsync(string columnName, UpdateColumnRequest request, CancellationToken cancellationToken = default)
        {
            var targetTable = string.IsNullOrWhiteSpace(request.TableName) ? "Users" : request.TableName;
            var existing = await _columnRepository.GetByNameAsync(targetTable, columnName);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Column '{columnName}' not found in table '{targetTable}'.");
            }

            var updatedCol = new Column
            {
                ColumnId = existing.ColumnId,
                Name = string.IsNullOrWhiteSpace(request.NewName) ? existing.Name : request.NewName,
                DataType = string.IsNullOrWhiteSpace(request.DataType) ? existing.DataType : ParseDataType(request.DataType),
                Nullable = request.IsNullable ?? existing.Nullable,
                DefaultValue = request.DefaultValue ?? existing.DefaultValue
            };

            var result = await _columnRepository.UpdateAsync(targetTable, columnName, updatedCol, cancellationToken);
            return MapToDto(result, targetTable);
        }

        public async Task<bool> DropColumnAsync(string tableName, string columnName, CancellationToken cancellationToken = default)
        {
            return await _columnRepository.DropAsync(tableName, columnName, cancellationToken);
        }

        private static DataTypeEnum ParseDataType(string typeStr)
        {
            if (Enum.TryParse<DataTypeEnum>(typeStr, true, out var type))
            {
                return type;
            }
            return DataTypeEnum.VARCHAR;
        }

        private static ColumnDto MapToDto(Column col, string tableName)
        {
            return new ColumnDto
            {
                ColumnId = col.ColumnId,
                Name = col.Name,
                TableName = tableName,
                DataType = col.DataType.ToString(),
                IsNullable = col.Nullable,
                DefaultValue = col.DefaultValue?.ToString()
            };
        }
    }
}
