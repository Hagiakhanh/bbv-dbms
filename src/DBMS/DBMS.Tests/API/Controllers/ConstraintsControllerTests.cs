using DBMS.API.Controllers;
using DBMS.API.DTOs.Constraints;
using DBMS.API.Services.Constraints;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Controllers
{
    public class ConstraintsControllerTests
    {
        private readonly Mock<IConstraintService> _mockService;
        private readonly ConstraintsController _controller;

        public ConstraintsControllerTests()
        {
            _mockService = new Mock<IConstraintService>();
            _controller = new ConstraintsController(_mockService.Object);
        }

        [Fact]
        public async Task AddConstraint_ShouldReturn201Created()
        {
            var req = new CreateConstraintRequest { Name = "PK_Users", Type = "PRIMARY_KEY" };
            var dto = new ConstraintDto { Name = "PK_Users", TableName = "Users", SchemaName = "dbo", DatabaseName = "master" };
            _mockService.Setup(s => s.AddConstraintAsync("master", "dbo", "Users", req, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _controller.AddConstraint("Users", req, "master", "dbo", CancellationToken.None);

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task GetConstraints_ShouldReturn200OK()
        {
            var list = new List<ConstraintDto> { new ConstraintDto { Name = "PK_Users" } };
            _mockService.Setup(s => s.GetConstraintsByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(list);

            var result = await _controller.GetConstraints("Users", "master", "dbo", CancellationToken.None);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var val = okResult.Value.Should().BeAssignableTo<IEnumerable<ConstraintDto>>().Subject;
            val.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetConstraintByName_ShouldReturn200OK_WhenFound()
        {
            var dto = new ConstraintDto { Name = "PK_Users" };
            _mockService.Setup(s => s.GetConstraintByNameAsync("master", "dbo", "Users", "PK_Users", It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _controller.GetConstraintByName("Users", "PK_Users", "master", "dbo", CancellationToken.None);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var val = okResult.Value.Should().BeOfType<ConstraintDto>().Subject;
            val.Name.Should().Be("PK_Users");
        }

        [Fact]
        public async Task DropConstraint_ShouldReturn204NoContent()
        {
            _mockService.Setup(s => s.DropConstraintAsync("master", "dbo", "Users", "PK_Users", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _controller.DropConstraint("Users", "PK_Users", "master", "dbo", CancellationToken.None);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
