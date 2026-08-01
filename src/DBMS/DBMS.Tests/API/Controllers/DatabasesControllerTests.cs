using DBMS.API.Controllers;
using DBMS.API.DTOs.Databases;
using ApiIDatabaseService = DBMS.API.Services.Databases.IDatabaseService;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Controllers
{
    public class DatabasesControllerTests
    {
        private readonly Mock<ApiIDatabaseService> _mockService;
        private readonly DatabasesController _controller;

        public DatabasesControllerTests()
        {
            _mockService = new Mock<ApiIDatabaseService>();
            _controller = new DatabasesController(_mockService.Object);
        }

        [Fact]
        public async Task GetDatabases_ShouldReturn200OKWithList()
        {
            // Arrange
            var dbs = new List<DatabaseDto>
            {
                new DatabaseDto { DatabaseId = 1, Name = "master", Owner = "sa" }
            };
            _mockService.Setup(s => s.GetAllDatabasesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(dbs);

            // Act
            var result = await _controller.GetDatabases(CancellationToken.None);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var list = okResult.Value.Should().BeAssignableTo<IEnumerable<DatabaseDto>>().Subject;
            list.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetDatabaseByName_ShouldReturn200OK_WhenExists()
        {
            // Arrange
            var db = new DatabaseDto { DatabaseId = 1, Name = "master", Owner = "sa" };
            _mockService.Setup(s => s.GetDatabaseByNameAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(db);

            // Act
            var result = await _controller.GetDatabaseByName("master", CancellationToken.None);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDb = okResult.Value.Should().BeOfType<DatabaseDto>().Subject;
            returnedDb.Name.Should().Be("master");
        }

        [Fact]
        public async Task GetDatabaseByName_ShouldReturn404NotFound_WhenNotExists()
        {
            // Arrange
            _mockService.Setup(s => s.GetDatabaseByNameAsync("GhostDb", It.IsAny<CancellationToken>())).ReturnsAsync((DatabaseDto?)null);

            // Act
            var result = await _controller.GetDatabaseByName("GhostDb", CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateDatabase_ShouldReturn201Created_WhenValid()
        {
            // Arrange
            var request = new CreateDatabaseRequest { Name = "SalesDb", Owner = "sa" };
            var createdDto = new DatabaseDto { DatabaseId = 2, Name = "SalesDb", Owner = "sa" };
            _mockService.Setup(s => s.CreateDatabaseAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.CreateDatabase(request, CancellationToken.None);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
            var value = createdResult.Value.Should().BeOfType<DatabaseDto>().Subject;
            value.Name.Should().Be("SalesDb");
        }

        [Fact]
        public async Task DropDatabase_ShouldReturn204NoContent_WhenSuccessful()
        {
            // Arrange
            _mockService.Setup(s => s.DropDatabaseAsync("TempDb", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.DropDatabase("TempDb", CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task SetDatabaseState_ShouldReturn200OK()
        {
            var req = new SetDatabaseStateRequest { State = "OFFLINE" };
            var dto = new DatabaseDto { Name = "master", State = "OFFLINE" };
            _mockService.Setup(s => s.SetStateAsync("master", req, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _controller.SetDatabaseState("master", req, CancellationToken.None);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var val = okResult.Value.Should().BeOfType<DatabaseDto>().Subject;
            val.State.Should().Be("OFFLINE");
        }

        [Fact]
        public async Task AttachDatabase_ShouldReturn201Created()
        {
            var req = new AttachDatabaseRequest { Name = "AttachedDb", FilePath = "/db.bin" };
            var dto = new DatabaseDto { Name = "AttachedDb" };
            _mockService.Setup(s => s.AttachDatabaseAsync(req, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _controller.AttachDatabase(req, CancellationToken.None);

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task DetachDatabase_ShouldReturn204NoContent()
        {
            _mockService.Setup(s => s.DetachDatabaseAsync("DetachDb", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.DetachDatabase("DetachDb", CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}

