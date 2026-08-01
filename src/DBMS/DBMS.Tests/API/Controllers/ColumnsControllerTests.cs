using DBMS.API.Controllers;
using DBMS.API.DTOs.Columns;
using DBMS.API.Services.Columns;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Controllers
{
    public class ColumnsControllerTests
    {
        private readonly Mock<IColumnService> _mockService;
        private readonly ColumnsController _controller;

        public ColumnsControllerTests()
        {
            _mockService = new Mock<IColumnService>();
            _controller = new ColumnsController(_mockService.Object);
        }

        [Fact]
        public async Task GetColumns_ShouldReturn200OKWithList()
        {
            // Arrange
            var list = new List<ColumnDto>
            {
                new ColumnDto { ColumnId = 1, Name = "Id", TableName = "Users", DataType = "INT" }
            };
            _mockService.Setup(s => s.GetColumnsByTableAsync("Users", It.IsAny<CancellationToken>())).ReturnsAsync(list);

            // Act
            var result = await _controller.GetColumns("Users", CancellationToken.None);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedList = okResult.Value.Should().BeAssignableTo<IEnumerable<ColumnDto>>().Subject;
            returnedList.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddColumn_ShouldReturn201Created_WhenValid()
        {
            // Arrange
            var request = new CreateColumnRequest { TableName = "Users", Name = "Email", DataType = "VARCHAR" };
            var dto = new ColumnDto { ColumnId = 2, Name = "Email", TableName = "Users", DataType = "VARCHAR" };
            _mockService.Setup(s => s.CreateColumnAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            var result = await _controller.AddColumn("Users", request, CancellationToken.None);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task DropColumn_ShouldReturn204NoContent_WhenSuccessful()
        {
            // Arrange
            _mockService.Setup(s => s.DropColumnAsync("Users", "Email", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.DropColumn("Users", "Email", CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
