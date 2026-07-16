using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.DatabaseObjectManagement;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.DatabaseObjectManagement;

public class MetadataCatalogTests
{
    private readonly Mock<IBufferPool> _mockBufferPool;
    private readonly MetadataCatalog _catalog;

    public MetadataCatalogTests()
    {
        _mockBufferPool = new Mock<IBufferPool>();
        _catalog = new MetadataCatalog(_mockBufferPool.Object);
    }

    [Fact]
    public void GetTableMeta_ShouldReturnTableMeta_WhenExists()
    {
        // Arrange
        var tableName = new TableName();

        // Act
        var result = _catalog.GetTableMeta(tableName);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetIndexMeta_ShouldReturnIndexMeta_WhenExists()
    {
        // Arrange
        var indexId = new IndexId();

        // Act
        var result = _catalog.GetIndexMeta(indexId);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void UpdateMeta_ShouldUpdateCatalogObject()
    {
        // Arrange
        var obj = new CatalogObject();

        // Act
        var act = () => _catalog.UpdateMeta(obj);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DeleteMeta_ShouldRemoveCatalogObject()
    {
        // Arrange
        var objId = new ObjectId();

        // Act
        var act = () => _catalog.DeleteMeta(objId);

        // Assert
        act.Should().NotThrow();
    }
}
