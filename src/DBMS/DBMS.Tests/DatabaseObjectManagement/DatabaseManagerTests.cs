using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.DatabaseObjectManagement;
using DBMS.Domain.Models;

namespace DBMS.Tests.DatabaseObjectManagement;

public class DatabaseManagerTests
{
    private readonly DatabaseManager _manager;

    public DatabaseManagerTests()
    {
        _manager = new DatabaseManager();
    }

    [Fact]
    public void CreateDatabase_ShouldCreateDatabaseSuccessfully()
    {
        // Arrange
        var dbName = new DatabaseName("TestDb");

        // Act
        var act = () => _manager.CreateDatabase(dbName);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DropDatabase_ShouldDropDatabaseSuccessfully()
    {
        // Arrange
        var dbName = new DatabaseName("TestDb");

        // Act
        var act = () => _manager.DropDatabase(dbName);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetDatabase_ShouldReturnDatabaseMeta_WhenExists()
    {
        // Arrange
        var dbName = new DatabaseName("TestDb");

        // Act
        var result = _manager.GetDatabase(dbName);

        // Assert
        result.Should().NotBeNull();
    }
}
