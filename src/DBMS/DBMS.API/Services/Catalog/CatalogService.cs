using DBMS.API.DTOs.Catalog;
using DBMS.API.DTOs.Columns;
using DBMS.API.Repositories.Columns;
using DBMS.API.Repositories.Constraints;
using DBMS.API.Repositories.Databases;
using DBMS.API.Repositories.Indexes;
using DBMS.API.Repositories.Schemas;
using DBMS.API.Repositories.Tables;

namespace DBMS.API.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ISchemaRepository _schemaRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IColumnRepository _columnRepository;
        private readonly IConstraintRepository _constraintRepository;
        private readonly IIndexRepository _indexRepository;

        public CatalogService(
            IDatabaseRepository databaseRepository,
            ISchemaRepository schemaRepository,
            ITableRepository tableRepository,
            IColumnRepository columnRepository,
            IConstraintRepository constraintRepository,
            IIndexRepository indexRepository)
        {
            _databaseRepository = databaseRepository;
            _schemaRepository = schemaRepository;
            _tableRepository = tableRepository;
            _columnRepository = columnRepository;
            _constraintRepository = constraintRepository;
            _indexRepository = indexRepository;
        }

        public async Task<IEnumerable<CatalogTreeNodeDto>> GetCatalogTreeAsync(string? databaseName = null, int? depth = null, CancellationToken cancellationToken = default)
        {
            var maxDepth = depth ?? 4;
            var databases = await _databaseRepository.GetAllAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                databases = databases.Where(d => d.Name.Equals(databaseName, StringComparison.OrdinalIgnoreCase));
            }

            var tree = new List<CatalogTreeNodeDto>();

            foreach (var db in databases)
            {
                var dbState = await _databaseRepository.GetStateAsync(db.Name, cancellationToken);
                var dbNode = new CatalogTreeNodeDto
                {
                    Id = $"db:{db.Name}",
                    Name = db.Name,
                    NodeType = "Database",
                    Metadata = new Dictionary<string, object>
                    {
                        { "DatabaseId", db.DatabaseId },
                        { "Owner", db.Owner },
                        { "State", dbState }
                    }
                };

                if (maxDepth >= 2)
                {
                    var schemas = await _schemaRepository.GetByDatabaseAsync(db.Name, cancellationToken);
                    foreach (var schema in schemas)
                    {
                        var schemaNode = new CatalogTreeNodeDto
                        {
                            Id = $"schema:{db.Name}.{schema.Name}",
                            Name = schema.Name,
                            NodeType = "Schema",
                            ParentId = dbNode.Id
                        };

                        if (maxDepth >= 3)
                        {
                            var tables = await _tableRepository.GetBySchemaAsync(db.Name, schema.Name, cancellationToken);
                            foreach (var table in tables)
                            {
                                var tableNode = new CatalogTreeNodeDto
                                {
                                    Id = $"table:{db.Name}.{schema.Name}.{table.Name}",
                                    Name = table.Name,
                                    NodeType = "Table",
                                    ParentId = schemaNode.Id,
                                    Metadata = new Dictionary<string, object>
                                    {
                                        { "TableId", table.TableId }
                                    }
                                };

                                if (maxDepth >= 4)
                                {
                                    var columns = await _columnRepository.GetByTableAsync(table.Name, cancellationToken);
                                    foreach (var col in columns)
                                    {
                                        tableNode.Children.Add(new CatalogTreeNodeDto
                                        {
                                            Id = $"column:{db.Name}.{schema.Name}.{table.Name}.{col.Name}",
                                            Name = col.Name,
                                            NodeType = "Column",
                                            ParentId = tableNode.Id,
                                            Metadata = new Dictionary<string, object>
                                            {
                                                { "DataType", col.DataType.ToString() },
                                                { "IsNullable", col.Nullable },
                                                { "DefaultValue", col.DefaultValue ?? string.Empty }
                                            }
                                        });
                                    }

                                    var constraints = await _constraintRepository.GetByTableAsync(db.Name, schema.Name, table.Name, cancellationToken);
                                    foreach (var con in constraints)
                                    {
                                        tableNode.Children.Add(new CatalogTreeNodeDto
                                        {
                                            Id = $"constraint:{db.Name}.{schema.Name}.{table.Name}.{con.Name}",
                                            Name = con.Name,
                                            NodeType = "Constraint",
                                            ParentId = tableNode.Id,
                                            Metadata = new Dictionary<string, object>
                                            {
                                                { "ConstraintType", con.Type }
                                            }
                                        });
                                    }

                                    var indexes = await _indexRepository.GetByTableAsync(db.Name, schema.Name, table.Name, cancellationToken);
                                    foreach (var idx in indexes)
                                    {
                                        tableNode.Children.Add(new CatalogTreeNodeDto
                                        {
                                            Id = $"index:{db.Name}.{schema.Name}.{table.Name}.{idx.Name}",
                                            Name = idx.Name,
                                            NodeType = "Index",
                                            ParentId = tableNode.Id,
                                            Metadata = new Dictionary<string, object>
                                            {
                                                { "IsUnique", idx.IsUnique },
                                                { "IsEnabled", idx.IsEnabled }
                                            }
                                        });
                                    }
                                }

                                schemaNode.Children.Add(tableNode);
                            }
                        }

                        dbNode.Children.Add(schemaNode);
                    }
                }

                tree.Add(dbNode);
            }

            return tree;
        }

        public async Task<DatabaseMetadataDto?> GetDatabaseMetadataAsync(string dbName, CancellationToken cancellationToken = default)
        {
            var db = await _databaseRepository.GetByNameAsync(dbName, cancellationToken);
            if (db == null) return null;

            var state = await _databaseRepository.GetStateAsync(dbName, cancellationToken);
            var schemas = await _schemaRepository.GetByDatabaseAsync(dbName, cancellationToken);

            var schemaSummaries = new List<SchemaSummaryDto>();
            int totalTables = 0;

            foreach (var s in schemas)
            {
                var tables = await _tableRepository.GetBySchemaAsync(dbName, s.Name, cancellationToken);
                var tableCount = tables.Count();
                totalTables += tableCount;

                schemaSummaries.Add(new SchemaSummaryDto
                {
                    Name = s.Name,
                    TableCount = tableCount
                });
            }

            return new DatabaseMetadataDto
            {
                DatabaseId = db.DatabaseId,
                Name = db.Name,
                Owner = db.Owner,
                State = state,
                SchemaCount = schemaSummaries.Count,
                TableCount = totalTables,
                Schemas = schemaSummaries
            };
        }

        public async Task<SchemaMetadataDto?> GetSchemaMetadataAsync(string schemaName, string? dbName = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(dbName) ? dbName : "master";
            var schema = await _schemaRepository.GetByNameAsync(schemaName, cancellationToken);
            if (schema == null) return null;

            var tables = await _tableRepository.GetBySchemaAsync(targetDb, schema.Name, cancellationToken);
            var tableSummaries = new List<TableSummaryDto>();

            foreach (var t in tables)
            {
                var columns = await _columnRepository.GetByTableAsync(t.Name, cancellationToken);
                tableSummaries.Add(new TableSummaryDto
                {
                    TableId = t.TableId,
                    Name = t.Name,
                    ColumnCount = columns.Count()
                });
            }

            return new SchemaMetadataDto
            {
                Name = schema.Name,
                DatabaseName = targetDb,
                TableCount = tableSummaries.Count,
                Tables = tableSummaries
            };
        }

        public async Task<TableMetadataDto?> GetTableMetadataAsync(string tableName, string? schemaName = null, string? dbName = null, CancellationToken cancellationToken = default)
        {
            var targetDb = !string.IsNullOrWhiteSpace(dbName) ? dbName : "master";
            var targetSchema = !string.IsNullOrWhiteSpace(schemaName) ? schemaName : "dbo";

            var table = await _tableRepository.GetByNameAsync(targetDb, targetSchema, tableName, cancellationToken);
            if (table == null) return null;

            var columns = await _columnRepository.GetByTableAsync(tableName, cancellationToken);
            var columnDtos = columns.Select(c => new ColumnDto
            {
                ColumnId = c.ColumnId,
                Name = c.Name,
                TableName = tableName,
                DataType = c.DataType.ToString(),
                IsNullable = c.Nullable,
                DefaultValue = c.DefaultValue?.ToString()
            }).ToList();

            var constraints = (await _constraintRepository.GetByTableAsync(targetDb, targetSchema, tableName, cancellationToken)).ToList();
            var indexes = (await _indexRepository.GetByTableAsync(targetDb, targetSchema, tableName, cancellationToken)).ToList();

            return new TableMetadataDto
            {
                TableId = table.TableId,
                Name = table.Name,
                SchemaName = targetSchema,
                DatabaseName = targetDb,
                Columns = columnDtos,
                Constraints = constraints,
                Indexes = indexes
            };
        }
    }
}
