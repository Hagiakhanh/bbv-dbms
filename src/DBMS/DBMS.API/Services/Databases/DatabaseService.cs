using DBMS.API.DTOs.Databases;
using DBMS.API.Repositories.Databases;
using DBMS.Domain.DatabaseObjects.Databases;

namespace DBMS.API.Services.Databases
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

            return await MapToDtoAsync(createdDb, cancellationToken);
        }

        public async Task<IEnumerable<DatabaseDto>> GetAllDatabasesAsync(CancellationToken cancellationToken = default)
        {
            var databases = await _databaseRepository.GetAllAsync(cancellationToken);
            var dtos = new List<DatabaseDto>();
            foreach (var db in databases)
            {
                dtos.Add(await MapToDtoAsync(db, cancellationToken));
            }
            return dtos;
        }

        public async Task<DatabaseDto?> GetDatabaseByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var database = await _databaseRepository.GetByNameAsync(name, cancellationToken);
            return database == null ? null : await MapToDtoAsync(database, cancellationToken);
        }

        public async Task<DatabaseDto> UpdateDatabaseAsync(string name, UpdateDatabaseRequest request, CancellationToken cancellationToken = default)
        {
            var updatedDb = await _databaseRepository.UpdateAsync(name, request.NewName, request.NewOwner, cancellationToken);
            return await MapToDtoAsync(updatedDb, cancellationToken);
        }

        public async Task<bool> DropDatabaseAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _databaseRepository.DropAsync(name, cancellationToken);
        }

        public async Task<DatabaseDto> SetStateAsync(string name, SetDatabaseStateRequest request, CancellationToken cancellationToken = default)
        {
            await _databaseRepository.SetStateAsync(name, request.State, cancellationToken);
            var db = await _databaseRepository.GetByNameAsync(name, cancellationToken);
            if (db == null)
            {
                throw new KeyNotFoundException($"Database '{name}' not found.");
            }
            return await MapToDtoAsync(db, cancellationToken);
        }

        public async Task<DatabaseDto> AttachDatabaseAsync(AttachDatabaseRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Database name is required.", nameof(request.Name));
            }

            await _databaseRepository.AttachAsync(request.Name, request.FilePath, cancellationToken);
            var db = await _databaseRepository.GetByNameAsync(request.Name, cancellationToken);
            return await MapToDtoAsync(db!, cancellationToken);
        }

        public async Task<bool> DetachDatabaseAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _databaseRepository.DetachAsync(name, cancellationToken);
        }

        private async Task<DatabaseDto> MapToDtoAsync(Database db, CancellationToken cancellationToken)
        {
            var state = await _databaseRepository.GetStateAsync(db.Name, cancellationToken);
            return new DatabaseDto
            {
                DatabaseId = db.DatabaseId,
                Name = db.Name,
                Owner = db.Owner,
                State = state,
                SchemaCount = db.Schemas?.Count ?? 0
            };
        }
    }
}

