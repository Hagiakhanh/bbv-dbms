using DBMS.Domain.DatabaseObjects.Columns;

namespace DBMS.API.Repositories.Columns
{
    public interface IColumnRepository
    {
        Task<Column> CreateAsync(string tableName, Column column, CancellationToken cancellationToken = default);
        Task<IEnumerable<Column>> GetByTableAsync(string tableName, CancellationToken cancellationToken = default);
        Task<Column?> GetByNameAsync(string tableName, string columnName, CancellationToken cancellationToken = default);
        Task<Column> UpdateAsync(string tableName, string columnName, Column updatedColumn, CancellationToken cancellationToken = default);
        Task<bool> DropAsync(string tableName, string columnName, CancellationToken cancellationToken = default);
    }
}
