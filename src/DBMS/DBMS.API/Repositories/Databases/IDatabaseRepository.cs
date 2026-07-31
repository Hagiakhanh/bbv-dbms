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
    }
}
