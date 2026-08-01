using DBMS.Domain.DatabaseObjects.Databases;

namespace DBMS.API.Repositories.Databases
{
    public interface IDatabaseRepository
    {
        Task<Database> CreateAsync(Database database, CancellationToken cancellationToken = default);
        Task<IEnumerable<Database>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Database?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<Database> UpdateAsync(string name, string? newName, string? newOwner, CancellationToken cancellationToken = default);
        Task<bool> DropAsync(string name, CancellationToken cancellationToken = default);
        Task SetStateAsync(string name, string state, CancellationToken cancellationToken = default);
        Task AttachAsync(string name, string filePath, CancellationToken cancellationToken = default);
        Task<bool> DetachAsync(string name, CancellationToken cancellationToken = default);
        Task<string> GetStateAsync(string name, CancellationToken cancellationToken = default);
    }
}

