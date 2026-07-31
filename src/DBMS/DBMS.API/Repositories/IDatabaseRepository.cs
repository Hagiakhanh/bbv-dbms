using DBMS.Domain.DatabaseObjects.Databases;

namespace DBMS.API.Repositories
{
    public interface IDatabaseRepository
    {
        Task<Database> CreateAsync(Database database, CancellationToken cancellationToken = default);
        Task<IEnumerable<Database>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Database?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
