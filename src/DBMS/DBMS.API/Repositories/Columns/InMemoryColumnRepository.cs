using DBMS.API.Storage;
using DBMS.Domain.Core;
using DBMS.Domain.DatabaseObjects.Columns;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Columns
{
    public class ColumnRecord
    {
        public int ColumnId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DataTypeEnum DataType { get; set; }
        public bool Nullable { get; set; }
        public string DefaultValue { get; set; } = string.Empty;
    }

    public class InMemoryColumnRepository : IColumnRepository
    {
        private const string FileName = "mock-columns.json";
        private readonly ConcurrentDictionary<string, (string TableName, Column Column)> _columns = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public InMemoryColumnRepository()
        {
            var defaultRecords = new List<ColumnRecord>
            {
                new ColumnRecord
                {
                    ColumnId = 1,
                    TableName = "Users",
                    Name = "Id",
                    DataType = DataTypeEnum.INT,
                    Nullable = false,
                    DefaultValue = "0"
                }
            };

            var records = JsonFileStorage.LoadAsync(FileName, defaultRecords).GetAwaiter().GetResult();
            foreach (var r in records)
            {
                var col = new Column
                {
                    ColumnId = r.ColumnId,
                    Name = r.Name,
                    DataType = r.DataType,
                    Nullable = r.Nullable,
                    DefaultValue = r.DefaultValue
                };

                _columns[$"{r.TableName}.{r.Name}"] = (r.TableName, col);
                if (r.ColumnId >= _nextId) _nextId = r.ColumnId + 1;
            }
        }

        private void Save()
        {
            var records = _columns.Values.Select(x => new ColumnRecord
            {
                ColumnId = x.Column.ColumnId,
                TableName = x.TableName,
                Name = x.Column.Name,
                DataType = x.Column.DataType,
                Nullable = x.Column.Nullable,
                DefaultValue = x.Column.DefaultValue?.ToString() ?? string.Empty
            }).ToList();

            _ = JsonFileStorage.SaveAsync(FileName, records);
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
            Save();
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
            Save();

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

            var removed = _columns.TryRemove(key, out _);
            if (removed) Save();
            return Task.FromResult(removed);
        }
    }
}

