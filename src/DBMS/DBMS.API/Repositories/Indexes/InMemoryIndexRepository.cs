using DBMS.API.DTOs.Indexes;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Indexes
{
    public class InMemoryIndexRepository : IIndexRepository
    {
        private readonly ConcurrentDictionary<string, IndexDto> _indexes = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public Task<IndexDto> CreateAsync(string databaseName, string schemaName, string tableName, CreateIndexRequest request, CancellationToken cancellationToken = default)
        {
            var key = GetKey(databaseName, schemaName, tableName, request.Name);
            if (_indexes.ContainsKey(key))
            {
                throw new InvalidOperationException($"Index '{request.Name}' already exists on table '{tableName}'.");
            }

            var dto = new IndexDto
            {
                IndexId = Interlocked.Increment(ref _nextId),
                Name = request.Name,
                Type = request.Type.ToUpperInvariant(),
                TableName = tableName,
                SchemaName = schemaName,
                DatabaseName = databaseName,
                Columns = request.Columns,
                IsUnique = request.IsUnique,
                IsEnabled = true
            };

            _indexes.TryAdd(key, dto);
            return Task.FromResult(dto);
        }

        public Task<IEnumerable<IndexDto>> GetByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var list = _indexes.Values
                .Where(x => x.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) &&
                            (string.IsNullOrWhiteSpace(databaseName) || x.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrWhiteSpace(schemaName) || x.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase)));

            return Task.FromResult(list);
        }

        public Task<IndexDto?> GetByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = FindKey(databaseName, schemaName, tableName, name);
            if (key != null && _indexes.TryGetValue(key, out var dto))
            {
                return Task.FromResult<IndexDto?>(dto);
            }

            return Task.FromResult<IndexDto?>(null);
        }

        public Task<bool> SetEnabledAsync(string databaseName, string schemaName, string tableName, string name, bool enabled, CancellationToken cancellationToken = default)
        {
            var key = FindKey(databaseName, schemaName, tableName, name);
            if (key == null || !_indexes.TryGetValue(key, out var dto)) return Task.FromResult(false);

            dto.IsEnabled = enabled;
            return Task.FromResult(true);
        }

        public Task<bool> RebuildAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = FindKey(databaseName, schemaName, tableName, name);
            if (key == null || !_indexes.TryGetValue(key, out var dto)) return Task.FromResult(false);

            dto.IsEnabled = true;
            return Task.FromResult(true);
        }

        public Task<bool> DropAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = FindKey(databaseName, schemaName, tableName, name);
            if (key == null) return Task.FromResult(false);

            return Task.FromResult(_indexes.TryRemove(key, out _));
        }

        private string? FindKey(string db, string schema, string table, string index)
        {
            return _indexes.Keys.FirstOrDefault(k =>
            {
                var item = _indexes[k];
                return item.Name.Equals(index, StringComparison.OrdinalIgnoreCase) &&
                       item.TableName.Equals(table, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static string GetKey(string db, string schema, string table, string index) => $"{db}.{schema}.{table}.{index}";
    }
}
