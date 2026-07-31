using DBMS.Domain.DatabaseObjects.Schemas;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Schemas
{
    public class InMemorySchemaRepository : ISchemaRepository
    {
        private readonly ConcurrentDictionary<string, (string DatabaseName, Schema Schema)> _schemas = new(StringComparer.OrdinalIgnoreCase);

        public InMemorySchemaRepository()
        {
            var defaultSchema = new Schema("dbo");
            _schemas.TryAdd("master.dbo", ("master", defaultSchema));
        }

        public Task<Schema> CreateAsync(string databaseName, Schema schema, CancellationToken cancellationToken = default)
        {
            var key = $"{databaseName}.{schema.Name}";

            if (_schemas.ContainsKey(key))
            {
                throw new InvalidOperationException($"Schema '{schema.Name}' already exists in database '{databaseName}'.");
            }

            _schemas.TryAdd(key, (databaseName, schema));
            return Task.FromResult(schema);
        }

        public Task<IEnumerable<Schema>> GetByDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            var schemas = _schemas.Values
                .Where(x => x.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Schema);

            return Task.FromResult(schemas);
        }

        public Task<Schema?> GetByNameAsync(string schemaName, CancellationToken cancellationToken = default)
        {
            var schema = _schemas.Values
                .Select(x => x.Schema)
                .FirstOrDefault(x => x.Name.Equals(schemaName, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(schema);
        }

        public Task<Schema> RenameAsync(string schemaName, string newName, CancellationToken cancellationToken = default)
        {
            var key = _schemas.Keys.FirstOrDefault(k => _schemas[k].Schema.Name.Equals(schemaName, StringComparison.OrdinalIgnoreCase));
            if (key == null)
            {
                throw new KeyNotFoundException($"Schema '{schemaName}' not found.");
            }

            var entry = _schemas[key];
            _schemas.TryRemove(key, out _);

            var newSchema = new Schema(newName);
            var newKey = $"{entry.DatabaseName}.{newName}";
            _schemas.TryAdd(newKey, (entry.DatabaseName, newSchema));

            return Task.FromResult(newSchema);
        }

        public Task<bool> DropAsync(string schemaName, CancellationToken cancellationToken = default)
        {
            var key = _schemas.Keys.FirstOrDefault(k => _schemas[k].Schema.Name.Equals(schemaName, StringComparison.OrdinalIgnoreCase));
            if (key == null) return Task.FromResult(false);

            return Task.FromResult(_schemas.TryRemove(key, out _));
        }

        public Task<bool> ExistsAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default)
        {
            var key = $"{databaseName}.{schemaName}";
            return Task.FromResult(_schemas.ContainsKey(key));
        }
    }
}
