using DBMS.API.Controllers;
using DBMS.API.DTOs.Tables;
using ApiITableService = DBMS.API.Services.Tables.ITableService;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Controllers
{
    public class TablesControllerTests
    {
        private readonly Mock<ApiITableService> _mockService;
        private readonly TablesController _controller;

        public TablesControllerTests()
        {
            _mockService = new Mock<ApiITableService>();
            _controller = new TablesController(_mockService.Object);
        }

        [Fact]
        public async Task GetTables_ShouldReturn200OKWithList()
        {
            var list = new List<TableDto>
            {
                new TableDto { TableId = 1, Name = "Users", SchemaName = "dbo", DatabaseName = "master" }
            };
            _mockService.Setup(s => s.GetTablesBySchemaAsync("master", "dbo", It.IsAny<CancellationToken>())).ReturnsAsync(list);

            var result = await _controller.GetTables("master", "dbo", null, null, CancellationToken.None);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedList = okResult.Value.Should().BeAssignableTo<IEnumerable<TableDto>>().Subject;
            returnedList.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateTable_ShouldReturn201Created_WhenValid()
        {
            var request = new CreateTableRequest { Name = "Users", SchemaName = "dbo", DatabaseName = "master" };
            var dto = new TableDto { TableId = 1, Name = "Users", SchemaName = "dbo", DatabaseName = "master" };
            _mockService.Setup(s => s.CreateTableAsync("master", "dbo", request, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _controller.CreateTable(request, "master", "dbo", CancellationToken.None);

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task DropTable_ShouldReturn204NoContent_WhenSuccessful()
        {
            _mockService.Setup(s => s.DropTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.DropTable("Users", "master", "dbo", CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
