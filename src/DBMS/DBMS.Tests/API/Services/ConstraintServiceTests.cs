using DBMS.API.DTOs.Constraints;
using DBMS.API.Repositories.Constraints;
using DBMS.API.Services.Constraints;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class ConstraintServiceTests
    {
        private readonly Mock<IConstraintRepository> _mockRepo;
        private readonly ConstraintService _service;

        public ConstraintServiceTests()
        {
            _mockRepo = new Mock<IConstraintRepository>();
            _service = new ConstraintService(_mockRepo.Object);
        }

        [Fact]
        public async Task AddConstraintAsync_ShouldReturnDto_WhenValid()
        {
            var req = new CreateConstraintRequest { Name = "PK_Users", Type = "PRIMARY_KEY", Columns = new List<string> { "Id" } };
            var dto = new ConstraintDto { ConstraintId = 1, Name = "PK_Users", Type = "PRIMARY_KEY", TableName = "Users", SchemaName = "dbo", DatabaseName = "master" };
            _mockRepo.Setup(r => r.CreateAsync("master", "dbo", "Users", req, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _service.AddConstraintAsync("master", "dbo", "Users", req);

            result.Should().NotBeNull();
            result.Name.Should().Be("PK_Users");
        }

        [Fact]
        public async Task AddConstraintAsync_ShouldThrowException_WhenNameEmpty()
        {
            var req = new CreateConstraintRequest { Name = "" };

            Func<Task> act = async () => await _service.AddConstraintAsync("master", "dbo", "Users", req);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetConstraintsByTableAsync_ShouldReturnList()
        {
            var list = new List<ConstraintDto> { new ConstraintDto { Name = "PK_Users" } };
            _mockRepo.Setup(r => r.GetByTableAsync("master", "dbo", "Users", It.IsAny<CancellationToken>())).ReturnsAsync(list);

            var result = await _service.GetConstraintsByTableAsync("master", "dbo", "Users");

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task DropConstraintAsync_ShouldReturnTrue_WhenSuccess()
        {
            _mockRepo.Setup(r => r.DropAsync("master", "dbo", "Users", "PK_Users", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.DropConstraintAsync("master", "dbo", "Users", "PK_Users");

            result.Should().BeTrue();
        }
    }
}
