using DBMS.Domain.DatabaseObjects.Tables;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Tables
{
    public class InMemoryTableRepository : ITableRepository
    {
        private readonly ConcurrentDictionary<string, (string DatabaseName, string SchemaName, Table Table)> _tables = new(StringComparer.OrdinalIgnoreCase);

        public Task<Table> CreateAsync(string databaseName, string schemaName, Table table, CancellationToken cancellationToken = default)
        {
            var key = GetKey(databaseName, schemaName, table.Name);
            if (_tables.ContainsKey(key))
            {
                throw new InvalidOperationException($"Table '{table.Name}' already exists in schema '{schemaName}' of database '{databaseName}'.");
            }

            _tables.TryAdd(key, (databaseName, schemaName, table));
            return Task.FromResult(table);
        }

        public Task<IEnumerable<Table>> GetBySchemaAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default)
        {
            var result = _tables.Values
                .Where(x => x.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase) &&
                            x.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Table);

            return Task.FromResult(result);
        }

        public Task<Table?> GetByNameAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var key = GetKey(databaseName, schemaName, tableName);
            if (_tables.TryGetValue(key, out var entry))
            {
                return Task.FromResult<Table?>(entry.Table);
            }

            var fallback = _tables.Values.FirstOrDefault(x => x.Table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult<Table?>(fallback.Table);
        }

        public Task<bool> ExistsAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var key = GetKey(databaseName, schemaName, tableName);
            if (_tables.ContainsKey(key)) return Task.FromResult(true);

            return Task.FromResult(_tables.Values.Any(x => x.Table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<Table> UpdateAsync(string databaseName, string schemaName, string tableName, string? newName, CancellationToken cancellationToken = default)
        {
            var key = _tables.Keys.FirstOrDefault(k =>
            {
                var entry = _tables[k];
                return entry.Table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase) &&
                       (string.IsNullOrWhiteSpace(databaseName) || entry.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrWhiteSpace(schemaName) || entry.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase));
            });

            if (key == null)
            {
                throw new KeyNotFoundException($"Table '{tableName}' not found.");
            }

            var existing = _tables[key];
            var targetName = string.IsNullOrWhiteSpace(newName) ? existing.Table.Name : newName;
            _tables.TryRemove(key, out _);

            var updatedTable = new Table(targetName);
            var newKey = GetKey(existing.DatabaseName, existing.SchemaName, targetName);
            _tables.TryAdd(newKey, (existing.DatabaseName, existing.SchemaName, updatedTable));

            return Task.FromResult(updatedTable);
        }

        public Task<bool> DropAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var key = _tables.Keys.FirstOrDefault(k =>
            {
                var entry = _tables[k];
                return entry.Table.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase) &&
                       (string.IsNullOrWhiteSpace(databaseName) || entry.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrWhiteSpace(schemaName) || entry.SchemaName.Equals(schemaName, StringComparison.OrdinalIgnoreCase));
            });

            if (key == null) return Task.FromResult(false);

            return Task.FromResult(_tables.TryRemove(key, out _));
        }

        private static string GetKey(string db, string schema, string table) => $"{db}.{schema}.{table}";
    }
}
