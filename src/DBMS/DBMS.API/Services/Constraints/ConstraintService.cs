using DBMS.API.DTOs.Constraints;
using DBMS.API.Repositories.Constraints;

namespace DBMS.API.Services.Constraints
{
    public class ConstraintService : IConstraintService
    {
        private readonly IConstraintRepository _constraintRepository;

        public ConstraintService(IConstraintRepository constraintRepository)
        {
            _constraintRepository = constraintRepository;
        }

        public async Task<ConstraintDto> AddConstraintAsync(string databaseName, string schemaName, string tableName, CreateConstraintRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Constraint name is required.", nameof(request.Name));
            }

            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _constraintRepository.CreateAsync(db, schema, tableName, request, cancellationToken);
        }

        public async Task<IEnumerable<ConstraintDto>> GetConstraintsByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _constraintRepository.GetByTableAsync(db, schema, tableName, cancellationToken);
        }

        public async Task<ConstraintDto?> GetConstraintByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _constraintRepository.GetByNameAsync(db, schema, tableName, name, cancellationToken);
        }

        public async Task<bool> DropConstraintAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _constraintRepository.DropAsync(db, schema, tableName, name, cancellationToken);
        }
    }
}
