using DBMS.API.DTOs.Constraints;

namespace DBMS.API.Services.Constraints
{
    public interface IConstraintService
    {
        Task<ConstraintDto> AddConstraintAsync(string databaseName, string schemaName, string tableName, CreateConstraintRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<ConstraintDto>> GetConstraintsByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default);
        Task<ConstraintDto?> GetConstraintByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
        Task<bool> DropConstraintAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default);
    }
}
