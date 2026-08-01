using DBMS.Domain.DatabaseObjects.Databases;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Databases
{
    public class InMemoryDatabaseRepository : IDatabaseRepository
    {
        private readonly ConcurrentDictionary<string, Database> _databases = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        private readonly ConcurrentDictionary<string, string> _states = new(StringComparer.OrdinalIgnoreCase);

        public InMemoryDatabaseRepository()
        {
            var defaultDb = new Database(1, "master", "sa");
            _databases.TryAdd("master", defaultDb);
            _states.TryAdd("master", "ONLINE");
            _nextId = 2;
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

            return Task.FromResult(updatedDb);
        }

        public Task<bool> DropAsync(string name, CancellationToken cancellationToken = default)
        {
            if (name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System database 'master' cannot be dropped.");
            }

            _states.TryRemove(name, out _);
            return Task.FromResult(_databases.TryRemove(name, out _));
        }

        public Task SetStateAsync(string name, string state, CancellationToken cancellationToken = default)
        {
            if (!_databases.ContainsKey(name))
            {
                throw new KeyNotFoundException($"Database '{name}' not found.");
            }

            _states[name] = state.ToUpperInvariant();
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
            return Task.CompletedTask;
        }

        public Task<bool> DetachAsync(string name, CancellationToken cancellationToken = default)
        {
            if (name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System database 'master' cannot be detached.");
            }

            _states.TryRemove(name, out _);
            return Task.FromResult(_databases.TryRemove(name, out _));
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

