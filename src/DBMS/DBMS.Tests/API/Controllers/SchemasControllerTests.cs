using DBMS.API.Controllers;
using DBMS.API.DTOs.Schemas;
using ApiISchemaService = DBMS.API.Services.Schemas.ISchemaService;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Controllers
{
    public class SchemasControllerTests
    {
        private readonly Mock<ApiISchemaService> _mockService;
        private readonly SchemasController _controller;

        public SchemasControllerTests()
        {
            _mockService = new Mock<ApiISchemaService>();
            _controller = new SchemasController(_mockService.Object);
        }

        [Fact]
        public async Task GetSchemas_ShouldReturn200OKWithList()
        {
            // Arrange
            var list = new List<SchemaDto>
            {
                new SchemaDto { SchemaId = 1, Name = "dbo", DatabaseName = "master" }
            };
            _mockService.Setup(s => s.GetSchemasByDatabaseAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(list);

            // Act
            var result = await _controller.GetSchemas("master", CancellationToken.None);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedList = okResult.Value.Should().BeAssignableTo<IEnumerable<SchemaDto>>().Subject;
            returnedList.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateSchema_ShouldReturn201Created_WhenValid()
        {
            // Arrange
            var request = new CreateSchemaRequest { DatabaseName = "master", Name = "public" };
            var dto = new SchemaDto { SchemaId = 2, Name = "public", DatabaseName = "master" };
            _mockService.Setup(s => s.CreateSchemaAsync("master", request, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            var result = await _controller.CreateSchema(request, CancellationToken.None);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }
    }
}
