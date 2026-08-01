using DBMS.API.DTOs.Constraints;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Constraints
{
    public class InMemoryConstraintRepository : IConstraintRepository
    {
        private readonly ConcurrentDictionary<string, ConstraintDto> _constraints = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public Task<ConstraintDto> CreateAsync(string databaseName, string schemaName, string tableName, CreateConstraintRequest request, CancellationToken cancellationToken = default)
        {
            var key = GetKey(databaseName, schemaName, tableName, request.Name);
            if (_constraints.ContainsKey(key))
            {
                throw new InvalidOperationException($"Constraint '{request.Name}' already exists on table '{tableName}'.");
            }

            var dto = new ConstraintDto
            {
                ConstraintId = Interlocked.Increment(ref _nextId),
                Name = request.Name,
                Type = request.Type.ToUpperInvariant(),
                TableName = tableName,
                SchemaName = schemaName,
                DatabaseName = databaseName,
                Columns = request.Columns,
                ReferenceTable = request.ReferenceTable,
                ReferenceColumns = request.ReferenceColumns,
                OnDelete = request.OnDelete,
                OnUpdate = request.OnUpdate,
                Expression = request.Expression
            };

            _constraints.TryAdd(key, dto);
            return Task.FromResult(dto);
        }

        public Task<IEnumerable<ConstraintDto>> GetByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var list = _constraints.Values
                .Where(x => x.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) &&
                            (string.IsNullOrWhiteSpace(databaseName) || x.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) &&
                            (string.IsNullOrWhiteSpace(schemaName) || x.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase)));

            return Task.FromResult(list);
        }

        public Task<ConstraintDto?> GetByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = _constraints.Keys.FirstOrDefault(k =>
            {
                var item = _constraints[k];
                return item.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                       item.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase);
            });

            if (key != null && _constraints.TryGetValue(key, out var dto))
            {
                return Task.FromResult<ConstraintDto?>(dto);
            }

            return Task.FromResult<ConstraintDto?>(null);
        }

        public Task<bool> DropAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var key = _constraints.Keys.FirstOrDefault(k =>
            {
                var item = _constraints[k];
                return item.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                       item.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase);
            });

            if (key == null) return Task.FromResult(false);

            return Task.FromResult(_constraints.TryRemove(key, out _));
        }

        private static string GetKey(string db, string schema, string table, string constraint) => $"{db}.{schema}.{table}.{constraint}";
    }
}
