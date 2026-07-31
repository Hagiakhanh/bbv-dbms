using DBMS.Domain.DatabaseObjects.Databases;
using System.Collections.Concurrent;

namespace DBMS.API.Repositories.Databases
{
    public class InMemoryDatabaseRepository : IDatabaseRepository
    {
        private readonly ConcurrentDictionary<string, Database> _databases = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public InMemoryDatabaseRepository()
        {
            var defaultDb = new Database(1, "master", "sa");
            _databases.TryAdd("master", defaultDb);
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
            var updatedDb = new Database(existingDb.DatabaseId, targetName, targetOwner);
            _databases.TryAdd(targetName, updatedDb);

            return Task.FromResult(updatedDb);
        }

        public Task<bool> DropAsync(string name, CancellationToken cancellationToken = default)
        {
            if (name.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("System database 'master' cannot be dropped.");
            }

            return Task.FromResult(_databases.TryRemove(name, out _));
        }
    }
}
