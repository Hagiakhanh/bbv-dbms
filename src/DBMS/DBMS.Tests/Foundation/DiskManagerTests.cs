using System;
using FluentAssertions;
using Xunit;
using DBMS.Domain.Foundation;

namespace DBMS.Tests.Foundation;

public class DiskManagerTests
{
    [Fact]
    public void Constructor_WithValidDbFilePath_ShouldCreateInstance()
    {
        // Arrange
        string validPath = "test_db.db";

        // Act
        var diskManager = new DiskManager(validPath);

        // Assert
        diskManager.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyFilePath_ShouldThrowArgumentException(string invalidPath)
    {
        // Arrange
        
        // Act
        Action act = () => new DiskManager(invalidPath);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
