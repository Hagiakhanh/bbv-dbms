using DBMS.API.DTOs.Indexes;
using DBMS.API.Repositories.Indexes;

namespace DBMS.API.Services.Indexes
{
    public class IndexService : IIndexService
    {
        private readonly IIndexRepository _indexRepository;

        public IndexService(IIndexRepository indexRepository)
        {
            _indexRepository = indexRepository;
        }

        public async Task<IndexDto> CreateIndexAsync(string databaseName, string schemaName, string tableName, CreateIndexRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Index name is required.", nameof(request.Name));
            }

            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.CreateAsync(db, schema, tableName, request, cancellationToken);
        }

        public async Task<IEnumerable<IndexDto>> GetIndexesByTableAsync(string databaseName, string schemaName, string tableName, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.GetByTableAsync(db, schema, tableName, cancellationToken);
        }

        public async Task<IndexDto?> GetIndexByNameAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.GetByNameAsync(db, schema, tableName, name, cancellationToken);
        }

        public async Task<bool> EnableIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.SetEnabledAsync(db, schema, tableName, name, true, cancellationToken);
        }

        public async Task<bool> DisableIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.SetEnabledAsync(db, schema, tableName, name, false, cancellationToken);
        }

        public async Task<bool> RebuildIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.RebuildAsync(db, schema, tableName, name, cancellationToken);
        }

        public async Task<bool> DropIndexAsync(string databaseName, string schemaName, string tableName, string name, CancellationToken cancellationToken = default)
        {
            var db = string.IsNullOrWhiteSpace(databaseName) ? "master" : databaseName;
            var schema = string.IsNullOrWhiteSpace(schemaName) ? "dbo" : schemaName;

            return await _indexRepository.DropAsync(db, schema, tableName, name, cancellationToken);
        }
    }
}
