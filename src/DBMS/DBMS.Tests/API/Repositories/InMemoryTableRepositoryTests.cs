using DBMS.API.Repositories.Tables;
using DBMS.Domain.DatabaseObjects.Tables;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories
{
    public class InMemoryTableRepositoryTests
    {
        private readonly InMemoryTableRepository _repository;

        public InMemoryTableRepositoryTests()
        {
            _repository = new InMemoryTableRepository();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTable_WhenValid()
        {
            var table = new Table("Orders");
            var created = await _repository.CreateAsync("master", "dbo", table);

            created.Should().NotBeNull();
            created.Name.Should().Be("Orders");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenDuplicateTable()
        {
            var table = new Table("Users");
            Func<Task> act = async () => await _repository.CreateAsync("master", "dbo", table);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetBySchemaAsync_ShouldReturnTables()
        {
            var tables = await _repository.GetBySchemaAsync("master", "dbo");

            tables.Should().NotBeNull();
            tables.Should().Contain(t => t.Name == "Users");
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnTable_WhenExists()
        {
            var table = await _repository.GetByNameAsync("master", "dbo", "Users");

            table.Should().NotBeNull();
            table!.Name.Should().Be("Users");
        }

        [Fact]
        public async Task UpdateAsync_ShouldRenameTable()
        {
            await _repository.CreateAsync("master", "dbo", new Table("Logins"));
            var updated = await _repository.UpdateAsync("master", "dbo", "Logins", "UserLogins");

            updated.Name.Should().Be("UserLogins");
        }

        [Fact]
        public async Task DropAsync_ShouldRemoveTable()
        {
            await _repository.CreateAsync("master", "dbo", new Table("TempTable"));
            var result = await _repository.DropAsync("master", "dbo", "TempTable");

            result.Should().BeTrue();
            var table = await _repository.GetByNameAsync("master", "dbo", "TempTable");
            table.Should().BeNull();
        }
    }
}
