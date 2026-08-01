using DBMS.API.DTOs.Indexes;
using DBMS.API.Storage;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Indexes
{
    public class InMemoryIndexRepository : IIndexRepository
    {
        private const string FileName = "mock-indexes.json";
        private readonly ConcurrentDictionary<string, IndexDto> _indexes = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public InMemoryIndexRepository()
        {
            var defaultRecords = new List<IndexDto>
            {
                new IndexDto
                {
                    IndexId = 1,
                    Name = "IX_Users_Id",
                    Type = "BTREE",
                    TableName = "Users",
                    SchemaName = "dbo",
                    DatabaseName = "master",
                    Columns = new List<string> { "Id" },
                    IsUnique = true,
                    IsEnabled = true
                }
            };
            var records = JsonFileStorage.LoadAsync(FileName, defaultRecords).GetAwaiter().GetResult();

            foreach (var dto in records)
            {
                var key = GetKey(dto.DatabaseName, dto.SchemaName, dto.TableName, dto.Name);
                _indexes[key] = dto;
                if (dto.IndexId >= _nextId) _nextId = dto.IndexId + 1;
            }
        }

        private void Save()
        {
            _ = JsonFileStorage.SaveAsync(FileName, _indexes.Values.ToList());
        }

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
            Save();
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
            Save();
            return Task.FromResult(true);
        }

        public Task<bool> RebuildAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = FindKey(databaseName, schemaName, tableName, name);
            if (key == null || !_indexes.TryGetValue(key, out var dto)) return Task.FromResult(false);

            dto.IsEnabled = true;
            Save();
            return Task.FromResult(true);
        }

        public Task<bool> DropAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = FindKey(databaseName, schemaName, tableName, name);
            if (key == null) return Task.FromResult(false);

            var removed = _indexes.TryRemove(key, out _);
            if (removed) Save();
            return Task.FromResult(removed);
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

