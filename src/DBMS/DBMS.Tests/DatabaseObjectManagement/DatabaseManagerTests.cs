using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.DatabaseObjectManagement;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.StorageEngine;

namespace DBMS.Tests.DatabaseObjectManagement;

public class DatabaseManagerTests
{
    private readonly Mock<IMetadataCatalog> _mockCatalog;
    private readonly DatabaseManager _manager;

    public DatabaseManagerTests()
    {
        _mockCatalog = new Mock<IMetadataCatalog>();
        // TablespaceManager doesn't have an interface yet, so passing null for red phase tests.
        _manager = new DatabaseManager(_mockCatalog.Object, null!);
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
