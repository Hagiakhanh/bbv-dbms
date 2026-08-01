using DBMS.API.Repositories.Databases;
using DBMS.Domain.DatabaseObjects.Databases;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories
{
    public class InMemoryDatabaseRepositoryTests
    {
        private readonly InMemoryDatabaseRepository _repository;

        public InMemoryDatabaseRepositoryTests()
        {
            _repository = new InMemoryDatabaseRepository();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddDatabase_WhenNameIsUnique()
        {
            // Arrange
            var db = new Database(0, "SalesDb", "sa");

            // Act
            var createdDb = await _repository.CreateAsync(db);

            // Assert
            createdDb.Should().NotBeNull();
            createdDb.DatabaseId.Should().BeGreaterThan(1);
            createdDb.Name.Should().Be("SalesDb");
            createdDb.Owner.Should().Be("sa");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenDatabaseAlreadyExists()
        {
            // Arrange
            var duplicateDb = new Database(0, "master", "sa");

            // Act
            Func<Task> act = async () => await _repository.CreateAsync(duplicateDb);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("*already exists*");
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnDatabase_WhenExists()
        {
            // Act
            var db = await _repository.GetByNameAsync("master");

            // Assert
            db.Should().NotBeNull();
            db!.Name.Should().Be("master");
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var db = await _repository.GetByNameAsync("NonExistentDb");

            // Assert
            db.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDatabaseNameAndOwner()
        {
            // Arrange
            await _repository.CreateAsync(new Database(0, "TempDb", "sa"));

            // Act
            var updated = await _repository.UpdateAsync("TempDb", "TempDb_V2", "admin");

            // Assert
            updated.Name.Should().Be("TempDb_V2");
            updated.Owner.Should().Be("admin");
        }

        [Fact]
        public async Task DropAsync_ShouldRemoveDatabase_WhenNotMaster()
        {
            // Arrange
            await _repository.CreateAsync(new Database(0, "TestDb", "sa"));

            // Act
            var result = await _repository.DropAsync("TestDb");

            // Assert
            result.Should().BeTrue();
            var db = await _repository.GetByNameAsync("TestDb");
            db.Should().BeNull();
        }

        [Fact]
        public async Task SetStateAsync_And_GetStateAsync_ShouldWork()
        {
            await _repository.SetStateAsync("master", "OFFLINE");
            var state = await _repository.GetStateAsync("master");
            state.Should().Be("OFFLINE");
        }

        [Fact]
        public async Task SetStateAsync_ShouldThrowKeyNotFound_WhenDbNotExists()
        {
            Func<Task> act = async () => await _repository.SetStateAsync("GhostDb", "OFFLINE");
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task AttachAsync_ShouldCreateDatabase()
        {
            await _repository.AttachAsync("AttachedDb", "/path/to/db");
            var db = await _repository.GetByNameAsync("AttachedDb");
            db.Should().NotBeNull();
            db!.Name.Should().Be("AttachedDb");
        }

        [Fact]
        public async Task DetachAsync_ShouldRemoveDatabase()
        {
            await _repository.AttachAsync("DetachDb", "/path/to/db");
            var result = await _repository.DetachAsync("DetachDb");
            result.Should().BeTrue();
            var db = await _repository.GetByNameAsync("DetachDb");
            db.Should().BeNull();
        }
    }
}

