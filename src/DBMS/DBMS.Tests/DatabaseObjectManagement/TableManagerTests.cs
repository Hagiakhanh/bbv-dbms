using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.DatabaseObjectManagement;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.StorageEngine;

namespace DBMS.Tests.DatabaseObjectManagement;

public class TableManagerTests
{
    private readonly Mock<IMetadataCatalog> _mockCatalog;
    private readonly TableManager _manager;

    public TableManagerTests()
    {
        _mockCatalog = new Mock<IMetadataCatalog>();
        // IndexManager and SpaceManager don't have interfaces yet, so passing null for red phase tests.
        _manager = new TableManager(_mockCatalog.Object, null!, null!);
    }

    [Fact]
    public void CreateTable_ShouldCreateTableSuccessfully()
    {
        // Arrange
        var def = new TableDef();

        // Act
        var act = () => _manager.CreateTable(def);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AlterTable_ShouldAlterTableSuccessfully()
    {
        // Arrange
        var name = new TableName();
        var patch = new TablePatch();

        // Act
        var act = () => _manager.AlterTable(name, patch);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DropTable_ShouldDropTableSuccessfully()
    {
        // Arrange
        var name = new TableName();

        // Act
        var act = () => _manager.DropTable(name);

        // Assert
        act.Should().NotThrow();
    }
}
