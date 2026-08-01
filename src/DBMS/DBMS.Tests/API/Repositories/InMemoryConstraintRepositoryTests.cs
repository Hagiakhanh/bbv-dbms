using DBMS.API.DTOs.Constraints;
using DBMS.API.Repositories.Constraints;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories
{
    public class InMemoryConstraintRepositoryTests
    {
        private readonly InMemoryConstraintRepository _repository;

        public InMemoryConstraintRepositoryTests()
        {
            _repository = new InMemoryConstraintRepository();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddConstraint()
        {
            var req = new CreateConstraintRequest
            {
                Name = "UQ_Email",
                Type = "UNIQUE",
                Columns = new List<string> { "Email" }
            };

            var created = await _repository.CreateAsync("master", "dbo", "Users", req);

            created.Should().NotBeNull();
            created.Name.Should().Be("UQ_Email");
            created.Type.Should().Be("UNIQUE");
        }

        [Fact]
        public async Task GetByTableAsync_ShouldReturnConstraints()
        {
            var list = await _repository.GetByTableAsync("master", "dbo", "Users");

            list.Should().NotBeNull();
            list.Should().Contain(c => c.Name == "PK_Users");
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnConstraint()
        {
            var item = await _repository.GetByNameAsync("master", "dbo", "Users", "PK_Users");

            item.Should().NotBeNull();
            item!.Name.Should().Be("PK_Users");
        }

        [Fact]
        public async Task DropAsync_ShouldRemoveConstraint()
        {
            var req = new CreateConstraintRequest { Name = "CK_Age", Type = "CHECK", Expression = "Age >= 18" };
            await _repository.CreateAsync("master", "dbo", "Users", req);

            var dropped = await _repository.DropAsync("master", "dbo", "Users", "CK_Age");

            dropped.Should().BeTrue();
            var item = await _repository.GetByNameAsync("master", "dbo", "Users", "CK_Age");
            item.Should().BeNull();
        }
    }
}
