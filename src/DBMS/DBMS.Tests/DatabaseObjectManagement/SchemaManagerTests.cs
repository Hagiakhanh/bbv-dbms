using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.DatabaseObjectManagement;
using DBMS.Domain.Models;

namespace DBMS.Tests.DatabaseObjectManagement;

public class SchemaManagerTests
{
    private readonly SchemaManager _manager;

    public SchemaManagerTests()
    {
        _manager = new SchemaManager();
    }

    [Fact]
    public void CreateSchema_ShouldCreateSchemaSuccessfully()
    {
        // Arrange
        var dbName = new DatabaseName("TestDb");
        var schemaName = "public";

        // Act
        var act = () => _manager.CreateSchema(dbName, schemaName);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DropSchema_ShouldDropSchemaSuccessfully()
    {
        // Arrange
        var dbName = new DatabaseName("TestDb");
        var schemaName = "public";

        // Act
        var act = () => _manager.DropSchema(dbName, schemaName);

        // Assert
        act.Should().NotThrow();
    }
}
