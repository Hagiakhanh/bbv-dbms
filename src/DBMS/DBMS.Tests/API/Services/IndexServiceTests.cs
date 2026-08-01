using DBMS.API.DTOs.Indexes;
using DBMS.API.Repositories.Indexes;
using DBMS.API.Services.Indexes;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class IndexServiceTests
    {
        private readonly Mock<IIndexRepository> _mockRepo;
        private readonly IndexService _service;

        public IndexServiceTests()
        {
            _mockRepo = new Mock<IIndexRepository>();
            _service = new IndexService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateIndexAsync_ShouldReturnDto_WhenValid()
        {
            var req = new CreateIndexRequest { Name = "IX_Users_Email", Type = "BTREE", Columns = new List<string> { "Email" } };
            var dto = new IndexDto { IndexId = 1, Name = "IX_Users_Email", Type = "BTREE", TableName = "Users", SchemaName = "dbo", DatabaseName = "master" };
            _mockRepo.Setup(r => r.CreateAsync("master", "dbo", "Users", req, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            var result = await _service.CreateIndexAsync("master", "dbo", "Users", req);

            result.Should().NotBeNull();
            result.Name.Should().Be("IX_Users_Email");
        }

        [Fact]
        public async Task CreateIndexAsync_ShouldThrowException_WhenNameEmpty()
        {
            var req = new CreateIndexRequest { Name = "" };

            Func<Task> act = async () => await _service.CreateIndexAsync("master", "dbo", "Users", req);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task EnableIndexAsync_ShouldReturnTrue()
        {
            _mockRepo.Setup(r => r.SetEnabledAsync("master", "dbo", "Users", "IX_Test", true, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.EnableIndexAsync("master", "dbo", "Users", "IX_Test");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DisableIndexAsync_ShouldReturnTrue()
        {
            _mockRepo.Setup(r => r.SetEnabledAsync("master", "dbo", "Users", "IX_Test", false, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.DisableIndexAsync("master", "dbo", "Users", "IX_Test");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task RebuildIndexAsync_ShouldReturnTrue()
        {
            _mockRepo.Setup(r => r.RebuildAsync("master", "dbo", "Users", "IX_Test", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.RebuildIndexAsync("master", "dbo", "Users", "IX_Test");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DropIndexAsync_ShouldReturnTrue()
        {
            _mockRepo.Setup(r => r.DropAsync("master", "dbo", "Users", "IX_Test", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.DropIndexAsync("master", "dbo", "Users", "IX_Test");

            result.Should().BeTrue();
        }
    }
}
