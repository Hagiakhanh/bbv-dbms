using DBMS.Domain.Core;
using DBMS.Domain.DatabaseObjects.Columns;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Columns
{
    public class InMemoryColumnRepository : IColumnRepository
    {
        // Key: "TableName.ColumnName", Value: (TableName, Column)
        private readonly ConcurrentDictionary<string, (string TableName, Column Column)> _columns = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public InMemoryColumnRepository()
        {
            // Seed sample column 'Id' for table 'Users'
            var defaultCol = new Column
            {
                ColumnId = 1,
                Name = "Id",
                DataType = DataTypeEnum.INT,
                Nullable = false,
                DefaultValue = "0"
            };
            _columns.TryAdd("Users.Id", ("Users", defaultCol));
            _nextId = 2;
        }

        public Task<Column> CreateAsync(string tableName, Column column, CancellationToken cancellationToken = default)
        {
            var key = $"{tableName}.{column.Name}";
            if (_columns.ContainsKey(key))
            {
                throw new InvalidOperationException($"Column '{column.Name}' already exists in table '{tableName}'.");
            }

            column.ColumnId = Interlocked.Increment(ref _nextId);
            _columns.TryAdd(key, (tableName, column));
            return Task.FromResult(column);
        }

        public Task<IEnumerable<Column>> GetByTableAsync(string tableName, CancellationToken cancellationToken = default)
        {
            var cols = _columns.Values
                .Where(x => x.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Column);

            return Task.FromResult(cols);
        }

        public Task<Column?> GetByNameAsync(string tableName, string columnName, CancellationToken cancellationToken = default)
        {
            var key = $"{tableName}.{columnName}";
            if (_columns.TryGetValue(key, out var entry))
            {
                return Task.FromResult<Column?>(entry.Column);
            }

            // Fallback: search by column name if table not specified
            var fallback = _columns.Values.FirstOrDefault(x => x.Column.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult<Column?>(fallback.Column);
        }

        public Task<Column> UpdateAsync(string tableName, string columnName, Column updatedColumn, CancellationToken cancellationToken = default)
        {
            var key = $"{tableName}.{columnName}";
            if (!_columns.TryGetValue(key, out var existing))
            {
                throw new KeyNotFoundException($"Column '{columnName}' not found in table '{tableName}'.");
            }

            _columns.TryRemove(key, out _);

            var newKey = $"{tableName}.{updatedColumn.Name}";
            _columns.TryAdd(newKey, (tableName, updatedColumn));

            return Task.FromResult(updatedColumn);
        }

        public Task<bool> DropAsync(string tableName, string columnName, CancellationToken cancellationToken = default)
        {
            var key = $"{tableName}.{columnName}";
            if (!_columns.ContainsKey(key))
            {
                var matchKey = _columns.Keys.FirstOrDefault(k => k.EndsWith($".{columnName}", StringComparison.OrdinalIgnoreCase));
                if (matchKey == null) return Task.FromResult(false);
                key = matchKey;
            }

            return Task.FromResult(_columns.TryRemove(key, out _));
        }
    }
}
