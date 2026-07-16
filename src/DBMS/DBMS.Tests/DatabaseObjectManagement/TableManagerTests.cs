using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.DatabaseObjectManagement;
using DBMS.Domain.Models;

namespace DBMS.Tests.DatabaseObjectManagement;

public class TableManagerTests
{
    private readonly TableManager _manager;

    public TableManagerTests()
    {
        _manager = new TableManager();
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
