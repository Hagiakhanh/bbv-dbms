using DBMS.Application.DTOs;
using DBMS.Domain.DatabaseObjects.Databases;
using DBMS.Domain.Interfaces;

namespace DBMS.Application.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDatabaseRepository _databaseRepository;

        public DatabaseService(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task<DatabaseDto> CreateDatabaseAsync(CreateDatabaseRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Database name is required.", nameof(request.Name));
            }

            var exists = await _databaseRepository.ExistsAsync(request.Name, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException($"Database '{request.Name}' already exists.");
            }

            var database = new Database(0, request.Name, string.IsNullOrWhiteSpace(request.Owner) ? "sa" : request.Owner);
            var createdDb = await _databaseRepository.CreateAsync(database, cancellationToken);

            return MapToDto(createdDb);
        }

        public async Task<IEnumerable<DatabaseDto>> GetAllDatabasesAsync(CancellationToken cancellationToken = default)
        {
            var databases = await _databaseRepository.GetAllAsync(cancellationToken);
            return databases.Select(MapToDto);
        }

        public async Task<DatabaseDto?> GetDatabaseByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var database = await _databaseRepository.GetByNameAsync(name, cancellationToken);
            return database == null ? null : MapToDto(database);
        }

        private static DatabaseDto MapToDto(Database db)
        {
            return new DatabaseDto
            {
                DatabaseId = db.DatabaseId,
                Name = db.Name,
                Owner = db.Owner,
                SchemaCount = db.Schemas?.Count ?? 0
            };
        }
    }
}
