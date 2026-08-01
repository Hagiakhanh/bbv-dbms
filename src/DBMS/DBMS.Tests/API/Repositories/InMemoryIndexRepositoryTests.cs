using DBMS.API.DTOs.Indexes;
using DBMS.API.Repositories.Indexes;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories
{
    public class InMemoryIndexRepositoryTests
    {
        private readonly InMemoryIndexRepository _repository;

        public InMemoryIndexRepositoryTests()
        {
            _repository = new InMemoryIndexRepository();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddIndex()
        {
            var req = new CreateIndexRequest
            {
                Name = "IX_Users_Email",
                Type = "BTREE",
                Columns = new List<string> { "Email" },
                IsUnique = true
            };

            var created = await _repository.CreateAsync("master", "dbo", "Users", req);

            created.Should().NotBeNull();
            created.Name.Should().Be("IX_Users_Email");
            created.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public async Task GetByTableAsync_ShouldReturnIndexes()
        {
            var list = await _repository.GetByTableAsync("master", "dbo", "Users");

            list.Should().NotBeNull();
            list.Should().Contain(i => i.Name == "IX_Users_Id");
        }

        [Fact]
        public async Task SetEnabledAsync_ShouldToggleStatus()
        {
            var disableResult = await _repository.SetEnabledAsync("master", "dbo", "Users", "IX_Users_Id", false);
            disableResult.Should().BeTrue();

            var idx = await _repository.GetByNameAsync("master", "dbo", "Users", "IX_Users_Id");
            idx!.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public async Task RebuildAsync_ShouldEnableIndex()
        {
            await _repository.SetEnabledAsync("master", "dbo", "Users", "IX_Users_Id", false);
            var rebuildResult = await _repository.RebuildAsync("master", "dbo", "Users", "IX_Users_Id");
            rebuildResult.Should().BeTrue();

            var idx = await _repository.GetByNameAsync("master", "dbo", "Users", "IX_Users_Id");
            idx!.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public async Task DropAsync_ShouldRemoveIndex()
        {
            var req = new CreateIndexRequest { Name = "IX_Temp", Type = "HASH", Columns = new List<string> { "TempCol" } };
            await _repository.CreateAsync("master", "dbo", "Users", req);

            var dropped = await _repository.DropAsync("master", "dbo", "Users", "IX_Temp");
            dropped.Should().BeTrue();

            var idx = await _repository.GetByNameAsync("master", "dbo", "Users", "IX_Temp");
            idx.Should().BeNull();
        }
    }
}
