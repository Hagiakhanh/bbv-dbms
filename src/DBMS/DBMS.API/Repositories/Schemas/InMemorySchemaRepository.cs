using DBMS.API.Storage;
using DBMS.Domain.DatabaseObjects.Schemas;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Schemas
{
    public class SchemaRecord
    {
        public string DatabaseName { get; set; } = "master";
        public string SchemaName { get; set; } = "dbo";
    }

    public class InMemorySchemaRepository : ISchemaRepository
    {
        private const string FileName = "mock-schemas.json";
        private readonly ConcurrentDictionary<string, (string DatabaseName, Schema Schema)> _schemas = new(StringComparer.OrdinalIgnoreCase);

        public InMemorySchemaRepository()
        {
            var defaultRecords = new List<SchemaRecord>
            {
                new SchemaRecord { DatabaseName = "master", SchemaName = "dbo" }
            };

            var records = JsonFileStorage.LoadAsync(FileName, defaultRecords).GetAwaiter().GetResult();
            foreach (var r in records)
            {
                var key = $"{r.DatabaseName}.{r.SchemaName}";
                _schemas[key] = (r.DatabaseName, new Schema(r.SchemaName));
            }
        }

        private void Save()
        {
            var records = _schemas.Values.Select(s => new SchemaRecord
            {
                DatabaseName = s.DatabaseName,
                SchemaName = s.Schema.Name
            }).ToList();

            _ = JsonFileStorage.SaveAsync(FileName, records);
        }

        public Task<Schema> CreateAsync(string databaseName, Schema schema, CancellationToken cancellationToken = default)
        {
            var key = $"{databaseName}.{schema.Name}";

            if (_schemas.ContainsKey(key))
            {
                throw new InvalidOperationException($"Schema '{schema.Name}' already exists in database '{databaseName}'.");
            }

            _schemas.TryAdd(key, (databaseName, schema));
            Save();
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
            Save();

            return Task.FromResult(newSchema);
        }

        public Task<bool> DropAsync(string schemaName, CancellationToken cancellationToken = default)
        {
            var key = _schemas.Keys.FirstOrDefault(k => _schemas[k].Schema.Name.Equals(schemaName, StringComparison.OrdinalIgnoreCase));
            if (key == null) return Task.FromResult(false);

            var removed = _schemas.TryRemove(key, out _);
            if (removed) Save();
            return Task.FromResult(removed);
        }

        public Task<bool> ExistsAsync(string databaseName, string schemaName, CancellationToken cancellationToken = default)
        {
            var key = $"{databaseName}.{schemaName}";
            return Task.FromResult(_schemas.ContainsKey(key));
        }
    }
}

