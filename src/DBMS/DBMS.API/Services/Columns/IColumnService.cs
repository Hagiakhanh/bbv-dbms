using DBMS.API.DTOs.Columns;

namespace DBMS.API.Services.Columns
{
    public interface IColumnService
    {
        Task<ColumnDto> CreateColumnAsync(CreateColumnRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<ColumnDto>> GetColumnsByTableAsync(string tableName, CancellationToken cancellationToken = default);
        Task<ColumnDto?> GetColumnByNameAsync(string tableName, string columnName, CancellationToken cancellationToken = default);
        Task<ColumnDto> UpdateColumnAsync(string columnName, UpdateColumnRequest request, CancellationToken cancellationToken = default);
        Task<bool> DropColumnAsync(string tableName, string columnName, CancellationToken cancellationToken = default);
    }
}
