using DBMS.API.Repositories.Schemas;
using DBMS.Domain.DatabaseObjects.Schemas;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories
{
    public class InMemorySchemaRepositoryTests
    {
        private readonly InMemorySchemaRepository _repository;

        public InMemorySchemaRepositoryTests()
        {
            _repository = new InMemorySchemaRepository();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddSchema_WhenValid()
        {
            // Arrange
            var schema = new Schema("public");

            // Act
            var created = await _repository.CreateAsync("master", schema);

            // Assert
            created.Should().NotBeNull();
            created.Name.Should().Be("public");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenSchemaAlreadyExistsInDatabase()
        {
            // Arrange (dbo already exists in master)
            var duplicateSchema = new Schema("dbo");

            // Act
            Func<Task> act = async () => await _repository.CreateAsync("master", duplicateSchema);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("*already exists*");
        }

        [Fact]
        public async Task GetByDatabaseAsync_ShouldReturnSchemasOfDatabase()
        {
            // Arrange
            await _repository.CreateAsync("master", new Schema("sales"));

            // Act
            var schemas = await _repository.GetByDatabaseAsync("master");

            // Assert
            schemas.Should().HaveCountGreaterThanOrEqualTo(2);
            schemas.Select(s => s.Name).Should().Contain(new[] { "dbo", "sales" });
        }

        [Fact]
        public async Task RenameAsync_ShouldUpdateSchemaName()
        {
            // Arrange
            await _repository.CreateAsync("master", new Schema("old_name"));

            // Act
            var renamed = await _repository.RenameAsync("old_name", "new_name");

            // Assert
            renamed.Name.Should().Be("new_name");
        }

        [Fact]
        public async Task DropAsync_ShouldRemoveSchema()
        {
            // Arrange
            await _repository.CreateAsync("master", new Schema("to_delete"));

            // Act
            var result = await _repository.DropAsync("to_delete");

            // Assert
            result.Should().BeTrue();
        }
    }
}
