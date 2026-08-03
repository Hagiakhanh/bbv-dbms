using DBMS.API.DTOs.Catalog;
using DBMS.API.DTOs.Constraints;
using DBMS.API.DTOs.Indexes;
using DBMS.API.Repositories.Columns;
using DBMS.API.Repositories.Constraints;
using DBMS.API.Repositories.Databases;
using DBMS.API.Repositories.Indexes;
using DBMS.API.Repositories.Schemas;
using DBMS.API.Repositories.Tables;
using DBMS.API.Services.Catalog;
using DBMS.Domain.Core;
using DBMS.Domain.DatabaseObjects.Columns;
using DBMS.Domain.DatabaseObjects.Databases;
using DBMS.Domain.DatabaseObjects.Schemas;
using DBMS.Domain.DatabaseObjects.Tables;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class CatalogServiceTests
    {
        private readonly Mock<IDatabaseRepository> _mockDbRepo;
        private readonly Mock<ISchemaRepository> _mockSchemaRepo;
        private readonly Mock<ITableRepository> _mockTableRepo;
        private readonly Mock<IColumnRepository> _mockColumnRepo;
        private readonly Mock<IConstraintRepository> _mockConstraintRepo;
        private readonly Mock<IIndexRepository> _mockIndexRepo;
        private readonly CatalogService _service;

        public CatalogServiceTests()
        {
            _mockDbRepo = new Mock<IDatabaseRepository>();
            _mockSchemaRepo = new Mock<ISchemaRepository>();
            _mockTableRepo = new Mock<ITableRepository>();
            _mockColumnRepo = new Mock<IColumnRepository>();
            _mockConstraintRepo = new Mock<IConstraintRepository>();
            _mockIndexRepo = new Mock<IIndexRepository>();

            _service = new CatalogService(
                _mockDbRepo.Object,
                _mockSchemaRepo.Object,
                _mockTableRepo.Object,
                _mockColumnRepo.Object,
                _mockConstraintRepo.Object,
                _mockIndexRepo.Object
            );
        }

        [Fact]
        public async Task GetCatalogTreeAsync_ShouldReturnHierarchicalTree()
        {
            // Arrange
            var databases = new List<Database> { new Database(1, "master", "sa") };
            var schemas = new List<Schema> { new Schema("dbo") };
            var tables = new List<Table> { new Table("Users") };
            var columns = new List<Column>
            {
                new Column { ColumnId = 100, Name = "Id", DataType = DataTypeEnum.INT, Nullable = false }
            };
            var constraints = new List<ConstraintDto>
            {
                new ConstraintDto { Name = "PK_Users", Type = "PRIMARY KEY", DatabaseName = "master", SchemaName = "dbo", TableName = "Users" }
            };
            var indexes = new List<IndexDto>
            {
                new IndexDto { Name = "IX_Users_Id", TableName = "Users", IsUnique = true, IsEnabled = true }
            };

            _mockDbRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(databases);
            _mockDbRepo.Setup(r => r.GetStateAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync("ONLINE");
            _mockSchemaRepo.Setup(r => r.GetByDatabaseAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(schemas);
            _mockTableRepo.Setup(r => r.GetBySchemaAsync("master", "dbo", It.IsAny<CancellationToken>())).ReturnsAsync(tables);
            _mockColumnRepo.Setup(r => r.GetByTableAsync("Users", It.IsAny<CancellationToken>())).ReturnsAsync(columns);
            _mockConstraintRepo.Setup(r => r.GetByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(constraints);
            _mockIndexRepo.Setup(r => r.GetByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(indexes);

            // Act
            var result = await _service.GetCatalogTreeAsync();

            // Assert
            result.Should().NotBeNull();
            var dbNodes = result.ToList();
            dbNodes.Should().HaveCount(1);
            dbNodes[0].Name.Should().Be("master");
            dbNodes[0].NodeType.Should().Be("Database");

            var schemaNodes = dbNodes[0].Children;
            schemaNodes.Should().HaveCount(1);
            schemaNodes[0].Name.Should().Be("dbo");
            schemaNodes[0].NodeType.Should().Be("Schema");

            var tableNodes = schemaNodes[0].Children;
            tableNodes.Should().HaveCount(1);
            tableNodes[0].Name.Should().Be("Users");
            tableNodes[0].NodeType.Should().Be("Table");

            var childNodes = tableNodes[0].Children;
            childNodes.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetDatabaseMetadataAsync_ShouldReturnDatabaseMetadata_WhenFound()
        {
            // Arrange
            var db = new Database(1, "master", "sa");
            var schemas = new List<Schema> { new Schema("dbo") };
            var tables = new List<Table> { new Table("Users") };

            _mockDbRepo.Setup(r => r.GetByNameAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(db);
            _mockDbRepo.Setup(r => r.GetStateAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync("ONLINE");
            _mockSchemaRepo.Setup(r => r.GetByDatabaseAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(schemas);
            _mockTableRepo.Setup(r => r.GetBySchemaAsync("master", "dbo", It.IsAny<CancellationToken>())).ReturnsAsync(tables);

            // Act
            var result = await _service.GetDatabaseMetadataAsync("master");

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("master");
            result.Owner.Should().Be("sa");
            result.State.Should().Be("ONLINE");
            result.SchemaCount.Should().Be(1);
            result.TableCount.Should().Be(1);
        }

        [Fact]
        public async Task GetSchemaMetadataAsync_ShouldReturnSchemaMetadata_WhenFound()
        {
            // Arrange
            var schema = new Schema("dbo");
            var tables = new List<Table> { new Table("Users") };
            var columns = new List<Column> { new Column { Name = "Id" } };

            _mockSchemaRepo.Setup(r => r.GetByNameAsync("dbo", It.IsAny<CancellationToken>())).ReturnsAsync(schema);
            _mockTableRepo.Setup(r => r.GetBySchemaAsync("master", "dbo", It.IsAny<CancellationToken>())).ReturnsAsync(tables);
            _mockColumnRepo.Setup(r => r.GetByTableAsync("Users", It.IsAny<CancellationToken>())).ReturnsAsync(columns);

            // Act
            var result = await _service.GetSchemaMetadataAsync("dbo", "master");

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("dbo");
            result.DatabaseName.Should().Be("master");
            result.TableCount.Should().Be(1);
            result.Tables[0].Name.Should().Be("Users");
        }

        [Fact]
        public async Task GetTableMetadataAsync_ShouldReturnTableMetadata_WhenFound()
        {
            // Arrange
            var table = new Table("Users");
            var columns = new List<Column>
            {
                new Column { ColumnId = 1, Name = "Id", DataType = DataTypeEnum.INT, Nullable = false }
            };
            var constraints = new List<ConstraintDto>
            {
                new ConstraintDto { Name = "PK_Users", Type = "PRIMARY KEY" }
            };
            var indexes = new List<IndexDto>
            {
                new IndexDto { Name = "IX_Users_Id" }
            };

            _mockTableRepo.Setup(r => r.GetByNameAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(table);
            _mockColumnRepo.Setup(r => r.GetByTableAsync("Users", It.IsAny<CancellationToken>())).ReturnsAsync(columns);
            _mockConstraintRepo.Setup(r => r.GetByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(constraints);
            _mockIndexRepo.Setup(r => r.GetByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(indexes);

            // Act
            var result = await _service.GetTableMetadataAsync("Users", "dbo", "master");

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Users");
            result.SchemaName.Should().Be("dbo");
            result.DatabaseName.Should().Be("master");
            result.Columns.Should().HaveCount(1);
            result.Constraints.Should().HaveCount(1);
            result.Indexes.Should().HaveCount(1);
        }
    }
}
