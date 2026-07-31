using DBMS.Domain.DatabaseObjects.Databases;
using DBMS.Domain.Interfaces;
using System.Collections.Concurrent;

namespace DBMS.Infrastructure.Persistence.Repositories
{
    public class InMemoryDatabaseRepository : IDatabaseRepository
    {
        private readonly ConcurrentDictionary<string, Database> _databases = new(StringComparer.OrdinalIgnoreCase);
        private int _nextId = 1;

        public InMemoryDatabaseRepository()
        {
            // Seed a default system database
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
    }
}
