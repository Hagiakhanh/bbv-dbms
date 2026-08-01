using DBMS.API.Repositories.Columns;
using DBMS.Domain.Core;
using DBMS.Domain.DatabaseObjects.Columns;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories
{
    public class InMemoryColumnRepositoryTests
    {
        private readonly InMemoryColumnRepository _repository;

        public InMemoryColumnRepositoryTests()
        {
            _repository = new InMemoryColumnRepository();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddColumnToTable()
        {
            // Arrange
            var col = new Column
            {
                Name = "Email",
                DataType = DataTypeEnum.VARCHAR,
                Nullable = true,
                DefaultValue = ""
            };

            // Act
            var created = await _repository.CreateAsync("Users", col);

            // Assert
            created.Should().NotBeNull();
            created.ColumnId.Should().BeGreaterThan(1);
            created.Name.Should().Be("Email");
        }

        [Fact]
        public async Task GetByTableAsync_ShouldReturnTableColumns()
        {
            // Arrange (Id already seeded for Users)
            await _repository.CreateAsync("Users", new Column { Name = "Age", DataType = DataTypeEnum.INT });

            // Act
            var cols = await _repository.GetByTableAsync("Users");

            // Assert
            cols.Should().HaveCountGreaterThanOrEqualTo(2);
            cols.Select(c => c.Name).Should().Contain(new[] { "Id", "Age" });
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyColumnAttributes()
        {
            // Arrange
            await _repository.CreateAsync("Users", new Column { Name = "Username", DataType = DataTypeEnum.VARCHAR });

            var updatedCol = new Column { ColumnId = 10, Name = "UserEmail", DataType = DataTypeEnum.VARCHAR, Nullable = true };

            // Act
            var result = await _repository.UpdateAsync("Users", "Username", updatedCol);

            // Assert
            result.Name.Should().Be("UserEmail");
        }

        [Fact]
        public async Task DropAsync_ShouldRemoveColumn()
        {
            // Arrange
            await _repository.CreateAsync("Users", new Column { Name = "TempCol", DataType = DataTypeEnum.INT });

            // Act
            var deleted = await _repository.DropAsync("Users", "TempCol");

            // Assert
            deleted.Should().BeTrue();
        }
    }
}
