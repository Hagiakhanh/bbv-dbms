using DBMS.API.DTOs.Schemas;
using DBMS.API.Repositories.Databases;
using DBMS.API.Repositories.Schemas;
using ApiSchemaService = DBMS.API.Services.Schemas.SchemaService;
using DBMS.Domain.DatabaseObjects.Schemas;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class SchemaServiceTests
    {
        private readonly Mock<ISchemaRepository> _mockSchemaRepo;
        private readonly Mock<IDatabaseRepository> _mockDbRepo;
        private readonly ApiSchemaService _service;

        public SchemaServiceTests()
        {
            _mockSchemaRepo = new Mock<ISchemaRepository>();
            _mockDbRepo = new Mock<IDatabaseRepository>();
            _service = new ApiSchemaService(_mockSchemaRepo.Object, _mockDbRepo.Object);
        }

        [Fact]
        public async Task CreateSchemaAsync_ShouldReturnSchemaDto_WhenValid()
        {
            // Arrange
            var request = new CreateSchemaRequest { DatabaseName = "master", Name = "public", Owner = "dbo" };
            _mockDbRepo.Setup(r => r.ExistsAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockSchemaRepo.Setup(r => r.CreateAsync("master", It.IsAny<Schema>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new Schema("public"));

            // Act
            var result = await _service.CreateSchemaAsync("master", request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("public");
            result.DatabaseName.Should().Be("master");
        }

        [Fact]
        public async Task CreateSchemaAsync_ShouldThrowKeyNotFoundException_WhenDatabaseNotExists()
        {
            // Arrange
            var request = new CreateSchemaRequest { DatabaseName = "UnknownDb", Name = "public" };
            _mockDbRepo.Setup(r => r.ExistsAsync("UnknownDb", It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _service.CreateSchemaAsync("UnknownDb", request);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                     .WithMessage("*UnknownDb*does not exist*");
        }
    }
}
