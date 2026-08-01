using DBMS.API.DTOs.Tables;
using DBMS.API.Repositories.Schemas;
using DBMS.API.Repositories.Tables;
using DBMS.API.Services.Tables;
using DBMS.Domain.DatabaseObjects.Tables;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class TableServiceTests
    {
        private readonly Mock<ITableRepository> _mockTableRepo;
        private readonly Mock<ISchemaRepository> _mockSchemaRepo;
        private readonly TableService _service;

        public TableServiceTests()
        {
            _mockTableRepo = new Mock<ITableRepository>();
            _mockSchemaRepo = new Mock<ISchemaRepository>();
            _service = new TableService(_mockTableRepo.Object, _mockSchemaRepo.Object);
        }

        [Fact]
        public async Task CreateTableAsync_ShouldReturnDto_WhenValid()
        {
            var req = new CreateTableRequest { Name = "Orders", SchemaName = "dbo", DatabaseName = "master" };
            var table = new Table("Orders");
            _mockTableRepo.Setup(r => r.CreateAsync("master", "dbo", It.IsAny<Table>(), It.IsAny<CancellationToken>())).ReturnsAsync(table);

            var result = await _service.CreateTableAsync("master", "dbo", req);

            result.Should().NotBeNull();
            result.Name.Should().Be("Orders");
            result.DatabaseName.Should().Be("master");
            result.SchemaName.Should().Be("dbo");
        }

        [Fact]
        public async Task CreateTableAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            var req = new CreateTableRequest { Name = "" };

            Func<Task> act = async () => await _service.CreateTableAsync("master", "dbo", req);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetTablesBySchemaAsync_ShouldReturnList()
        {
            var tables = new List<Table> { new Table("Users"), new Table("Products") };
            _mockTableRepo.Setup(r => r.GetBySchemaAsync("master", "dbo", It.IsAny<CancellationToken>())).ReturnsAsync(tables);

            var result = await _service.GetTablesBySchemaAsync("master", "dbo");

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task UpdateTableAsync_ShouldReturnUpdatedDto()
        {
            var req = new UpdateTableRequest { NewName = "RenamedTable" };
            var updated = new Table("RenamedTable");
            _mockTableRepo.Setup(r => r.UpdateAsync("master", "dbo", "OldTable", "RenamedTable", It.IsAny<CancellationToken>())).ReturnsAsync(updated);

            var result = await _service.UpdateTableAsync("master", "dbo", "OldTable", req);

            result.Should().NotBeNull();
            result.Name.Should().Be("RenamedTable");
        }

        [Fact]
        public async Task DropTableAsync_ShouldReturnTrue_WhenSuccess()
        {
            _mockTableRepo.Setup(r => r.DropAsync("master", "dbo", "TempTable", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.DropTableAsync("master", "dbo", "TempTable");

            result.Should().BeTrue();
        }
    }
}
