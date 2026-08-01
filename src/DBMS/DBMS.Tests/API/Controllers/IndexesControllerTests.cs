using DBMS.API.Controllers;
using DBMS.API.DTOs.Indexes;
using DBMS.API.Services.Indexes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Controllers
{
    public class IndexesControllerTests
    {
        private readonly Mock<IIndexService> _mockService;
        private readonly IndexesController _controller;

        public IndexesControllerTests()
        {
            _mockService = new Mock<IIndexService>();
            _controller = new IndexesController(_mockService.Object);
        }

        [Fact]
        public async Task CreateIndex_ShouldReturn201Created()
        {
            var req = new CreateIndexRequest { Name = "IX_Users_Email", Type = "BTREE" };
            var dto = new IndexDto { Name = "IX_Users_Email", TableName = "Users", SchemaName = "dbo", DatabaseName = "master" };
            _mockService.Setup(s => s.CreateIndexAsync("master", "dbo", "Users", req, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _controller.CreateIndex("Users", req, "master", "dbo", CancellationToken.None);

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task GetIndexes_ShouldReturn200OK()
        {
            var list = new List<IndexDto> { new IndexDto { Name = "IX_Users_Email" } };
            _mockService.Setup(s => s.GetIndexesByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(list);

            var result = await _controller.GetIndexes("Users", "master", "dbo", CancellationToken.None);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var val = okResult.Value.Should().BeAssignableTo<IEnumerable<IndexDto>>().Subject;
            val.Should().HaveCount(1);
        }

        [Fact]
        public async Task RebuildIndex_ShouldReturn202Accepted()
        {
            _mockService.Setup(s => s.RebuildIndexAsync("master", "dbo", "Users", "IX_Users_Email", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.RebuildIndex("Users", "IX_Users_Email", "master", "dbo", CancellationToken.None);

            result.Should().BeOfType<AcceptedResult>();
        }

        [Fact]
        public async Task EnableIndex_ShouldReturn200OK()
        {
            _mockService.Setup(s => s.EnableIndexAsync("master", "dbo", "Users", "IX_Users_Email", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.EnableIndex("Users", "IX_Users_Email", "master", "dbo", CancellationToken.None);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DisableIndex_ShouldReturn200OK()
        {
            _mockService.Setup(s => s.DisableIndexAsync("master", "dbo", "Users", "IX_Users_Email", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.DisableIndex("Users", "IX_Users_Email", "master", "dbo", CancellationToken.None);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DropIndex_ShouldReturn204NoContent()
        {
            _mockService.Setup(s => s.DropIndexAsync("master", "dbo", "Users", "IX_Users_Email", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.DropIndex("Users", "IX_Users_Email", "master", "dbo", CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
