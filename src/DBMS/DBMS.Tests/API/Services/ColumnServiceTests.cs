using DBMS.API.DTOs.Columns;
using DBMS.API.Repositories.Columns;
using DBMS.API.Services.Columns;
using DBMS.Domain.Core;
using DBMS.Domain.DatabaseObjects.Columns;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class ColumnServiceTests
    {
        private readonly Mock<IColumnRepository> _mockRepo;
        private readonly ColumnService _service;

        public ColumnServiceTests()
        {
            _mockRepo = new Mock<IColumnRepository>();
            _service = new ColumnService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateColumnAsync_ShouldReturnColumnDto_WhenValid()
        {
            // Arrange
            var request = new CreateColumnRequest
            {
                TableName = "Users",
                Name = "Email",
                DataType = "VARCHAR",
                IsNullable = true,
                DefaultValue = ""
            };

            var created = new Column { ColumnId = 2, Name = "Email", DataType = DataTypeEnum.VARCHAR, Nullable = true, DefaultValue = "" };
            _mockRepo.Setup(r => r.CreateAsync("Users", It.IsAny<Column>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(created);

            // Act
            var result = await _service.CreateColumnAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Email");
            result.DataType.Should().Be("VARCHAR");
            result.TableName.Should().Be("Users");
        }

        [Fact]
        public async Task CreateColumnAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            // Arrange
            var request = new CreateColumnRequest { TableName = "Users", Name = "" };

            // Act
            Func<Task> act = async () => await _service.CreateColumnAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage("*Column name is required*");
        }
    }
}
