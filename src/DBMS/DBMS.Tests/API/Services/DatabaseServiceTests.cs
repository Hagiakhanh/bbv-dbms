using DBMS.API.DTOs.Databases;
using DBMS.API.Repositories.Databases;
using ApiDatabaseService = DBMS.API.Services.Databases.DatabaseService;
using DBMS.Domain.DatabaseObjects.Databases;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.API.Services
{
    public class DatabaseServiceTests
    {
        private readonly Mock<IDatabaseRepository> _mockRepo;
        private readonly ApiDatabaseService _service;

        public DatabaseServiceTests()
        {
            _mockRepo = new Mock<IDatabaseRepository>();
            _service = new ApiDatabaseService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateDatabaseAsync_ShouldReturnDto_WhenValidRequest()
        {
            // Arrange
            var request = new CreateDatabaseRequest { Name = "SalesDb", Owner = "sa" };
            var createdDb = new Database(10, "SalesDb", "sa");

            _mockRepo.Setup(r => r.ExistsAsync("SalesDb", It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Database>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdDb);

            // Act
            var result = await _service.CreateDatabaseAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.DatabaseId.Should().Be(10);
            result.Name.Should().Be("SalesDb");
            result.Owner.Should().Be("sa");
        }

        [Fact]
        public async Task CreateDatabaseAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
        {
            // Arrange
            var request = new CreateDatabaseRequest { Name = "" };

            // Act
            Func<Task> act = async () => await _service.CreateDatabaseAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage("*Database name is required*");
        }

        [Fact]
        public async Task CreateDatabaseAsync_ShouldThrowInvalidOperationException_WhenAlreadyExists()
        {
            // Arrange
            var request = new CreateDatabaseRequest { Name = "SalesDb" };
            _mockRepo.Setup(r => r.ExistsAsync("SalesDb", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.CreateDatabaseAsync(request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("*already exists*");
        }

        [Fact]
        public async Task GetAllDatabasesAsync_ShouldReturnAllDatabaseDtos()
        {
            // Arrange
            var dbs = new List<Database>
            {
                new Database(1, "master", "sa"),
                new Database(2, "SalesDb", "admin")
            };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(dbs);

            // Act
            var result = await _service.GetAllDatabasesAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task SetStateAsync_ShouldUpdateStateAndReturnDto()
        {
            var db = new Database(1, "master", "sa");
            _mockRepo.Setup(r => r.SetStateAsync("master", "OFFLINE", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.GetByNameAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync(db);
            _mockRepo.Setup(r => r.GetStateAsync("master", It.IsAny<CancellationToken>())).ReturnsAsync("OFFLINE");

            var result = await _service.SetStateAsync("master", new SetDatabaseStateRequest { State = "OFFLINE" });

            result.Should().NotBeNull();
            result.State.Should().Be("OFFLINE");
        }

        [Fact]
        public async Task AttachDatabaseAsync_ShouldAttachAndReturnDto()
        {
            var req = new AttachDatabaseRequest { Name = "AttachedDb", FilePath = "/db.bin" };
            var db = new Database(5, "AttachedDb", "sa");
            _mockRepo.Setup(r => r.AttachAsync("AttachedDb", "/db.bin", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.GetByNameAsync("AttachedDb", It.IsAny<CancellationToken>())).ReturnsAsync(db);

            var result = await _service.AttachDatabaseAsync(req);

            result.Should().NotBeNull();
            result.Name.Should().Be("AttachedDb");
        }

        [Fact]
        public async Task DetachDatabaseAsync_ShouldReturnTrue_WhenSuccess()
        {
            _mockRepo.Setup(r => r.DetachAsync("DetachDb", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var result = await _service.DetachDatabaseAsync("DetachDb");

            result.Should().BeTrue();
        }
    }
}

