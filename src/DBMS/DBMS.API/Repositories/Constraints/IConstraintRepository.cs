using DBMS.API.DTOs.Constraints;

namespace DBMS.API.Repositories.Constraints
{
    public interface IConstraintRepository
    {
        Task<ConstraintDto> CreateAsync(string databaseName, string schemaName, string tableName, CreateConstraintRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<ConstraintDto>> GetByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<ConstraintDto?> GetByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> DropAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
    }
}
