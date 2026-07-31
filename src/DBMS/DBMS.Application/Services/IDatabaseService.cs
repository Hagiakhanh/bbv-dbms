using DBMS.Application.DTOs;

namespace DBMS.Application.Services
{
    public interface IDatabaseService
    {
        Task<DatabaseDto> CreateDatabaseAsync(CreateDatabaseRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<DatabaseDto>> GetAllDatabasesAsync(CancellationToken cancellationToken = default);
        Task<DatabaseDto?> GetDatabaseByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
