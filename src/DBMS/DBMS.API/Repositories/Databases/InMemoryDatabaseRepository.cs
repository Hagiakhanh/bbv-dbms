using DBMS.API.Storage;
using DBMS.Domain.DatabaseObjects.Databases;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Databases
{
    public class DatabaseRecord
    {
        public int DatabaseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = "sa";
        public string State { get; set; } = "ONLINE";
    }

    public class InMemoryDatabaseRepository : IDatabaseRepository
    {
        private const string FileName = "mock-databases.json";
        private readonly ConcurrentDictionary<string, Database> _databases = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, string> _states = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public InMemoryDatabaseRepository()
        {
            var defaultRecords = new List<DatabaseRecord>
            {
                new DatabaseRecord { DatabaseId = 1, Name = "master", Owner = "sa", State = "ONLINE" }
            };

            var records = JsonFileStorage.LoadAsync(FileName, defaultRecords).GetAwaiter().GetResult();
            foreach (var r in records)
            {
                _databases[r.Name] = new Database(r.DatabaseId, r.Name, r.Owner);
                _states[r.Name] = r.State;
                if (r.DatabaseId >= _nextId) _nextId = r.DatabaseId + 1;
            }
        }

        private void Save()
        {
            var records = _databases.Values.Select(db => new DatabaseRecord
            {
                DatabaseId = db.DatabaseId,
                Name = db.Name,
                Owner = db.Owner,
                State = _states.TryGetValue(db.Name, out var st) ? st : "ONLINE"
            }).ToList();

            _ = JsonFileStorage.SaveAsync(FileName, records);
        }

        public Task<Database> CreateAsync(Database database, CancellationToken cancellationToken = default)
        {
            if (_databases.ContainsKey(database.Name))
            {
                throw new InvalidOperationException($"Database '{database.Name}' already exists.");
            }

            var newDb = new Database(Interlocked.Increment(ref _nextId), database.Name, database.Owner);
            _databases.TryAdd(newDb.Name, newDb);
            _states.TryAdd(newDb.Name, "ONLINE");
            Save();
            return Task.FromResult(newDb);
        }

        public Task<IEnumerable<Database>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Database>>(_databases.Values);
        }

        public Task<Database?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            _databases.TryGetValue(name, out var database);
            return Task.FromResult(database);
        }

        public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_databases.ContainsKey(name));
        }

        public Task<Database> UpdateAsync(string name, string? newName, string? newOwner, CancellationToken cancellationToken = default)
        {
            if (!_databases.TryGetValue(name, out var existingDb))
            {
                throw new KeyNotFoundException($"Database '{name}' not found.");
            }

            var targetName = string.IsNullOrWhiteSpace(newName) ? existingDb.Name : newName;
            var targetOwner = string.IsNullOrWhiteSpace(newOwner) ? existingDb.Owner : newOwner;

            if (!targetName.Equals(name, StringComparison.OrdinalIgnoreCase) && _databases.ContainsKey(targetName))
            {
                throw new InvalidOperationException($"Database '{targetName}' already exists.");
            }

            _databases.TryRemove(name, out _);
            _states.TryRemove(name, out var currentState);

            var updatedDb = new Database(existingDb.DatabaseId, targetName, targetOwner);
            _databases.TryAdd(targetName, updatedDb);
            _states.TryAdd(targetName, currentState ?? "ONLINE");

            Save();
            return Task.FromResult(updatedDb);
        }

        public Task<bool> DropAsync(string name, CancellationToken cancellationToken = default)
        {
            if (name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System database 'master' cannot be dropped.");
            }

            _states.TryRemove(name, out _);
            var removed = _databases.TryRemove(name, out _);
            if (removed) Save();
            return Task.FromResult(removed);
        }

        public Task SetStateAsync(string name, string state, CancellationToken cancellationToken = default)
        {
            if (!_databases.ContainsKey(name))
            {
                throw new KeyNotFoundException($"Database '{name}' not found.");
            }

            _states[name] = state.ToUpperInvariant();
            Save();
            return Task.CompletedTask;
        }

        public Task AttachAsync(string name, string filePath, CancellationToken cancellationToken = default)
        {
            if (_databases.ContainsKey(name))
            {
                throw new InvalidOperationException($"Database '{name}' already exists.");
            }

            var attachedDb = new Database(Interlocked.Increment(ref _nextId), name, "sa");
            _databases.TryAdd(name, attachedDb);
            _states.TryAdd(name, "ONLINE");
            Save();
            return Task.CompletedTask;
        }

        public Task<bool> DetachAsync(string name, CancellationToken cancellationToken = default)
        {
            if (name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System database 'master' cannot be detached.");
            }

            _states.TryRemove(name, out _);
            var detached = _databases.TryRemove(name, out _);
            if (detached) Save();
            return Task.FromResult(detached);
        }

        public Task<string> GetStateAsync(string name, CancellationToken cancellationToken = default)
        {
            if (_states.TryGetValue(name, out var state))
            {
                return Task.FromResult(state);
            }

            return Task.FromResult("ONLINE");
        }
    }
}


