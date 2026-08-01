using DBMS.API.DTOs.Databases;

namespace DBMS.API.Services.Databases
{
    public interface IDatabaseService
    {
        Task<DatabaseDto> CreateDatabaseAsync(CreateDatabaseRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<DatabaseDto>> GetAllDatabasesAsync(CancellationToken cancellationToken = default);
        Task<DatabaseDto?> GetDatabaseByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<DatabaseDto> UpdateDatabaseAsync(string name, UpdateDatabaseRequest request, CancellationToken cancellationToken = default);
        Task<bool> DropDatabaseAsync(string name, CancellationToken cancellationToken = default);
        Task<DatabaseDto> SetStateAsync(string name, SetDatabaseStateRequest request, CancellationToken cancellationToken = default);
        Task<DatabaseDto> AttachDatabaseAsync(AttachDatabaseRequest request, CancellationToken cancellationToken = default);
        Task<bool> DetachDatabaseAsync(string name, CancellationToken cancellationToken = default);
    }
}

