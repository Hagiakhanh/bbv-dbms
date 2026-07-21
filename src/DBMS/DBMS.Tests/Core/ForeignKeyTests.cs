using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Core;

public class ForeignKeyTests
{
    private readonly Mock<Table> _mockReferenceTable;
    private readonly Mock<IRecordManager> _mockRecordManager;
    private readonly List<Column> _columns;
    private readonly ForeignKey _foreignKey;

    public ForeignKeyTests()
    {
        _mockReferenceTable = new Mock<Table>("Users");
        _mockRecordManager = new Mock<IRecordManager>();
        _columns = new List<Column> { new Column { Name = "RefId" } };

        _foreignKey = new ForeignKey("FK_Test", _mockReferenceTable.Object, _columns, _mockRecordManager.Object);
    }

    [Fact]
    public void ForeignKey_ShouldAcceptExistingReference()
    {
        // Arrange
        var row = new Row();
        var referencedRow = new Row();
        _mockReferenceTable.Setup(x => x.LookupReferencedRow(row)).Returns(referencedRow);

        // Act
        var result = _foreignKey.Validate(row);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ForeignKey_ShouldRejectMissingReference()
    {
        // Arrange
        var row = new Row();
        _mockReferenceTable.Setup(x => x.LookupReferencedRow(row)).Returns((Row)null);

        // Act
        Action act = () => _foreignKey.Validate(row);

        // Assert
        act.Should().Throw<ForeignKeyReferenceException>();
    }

    [Fact]
    public void ForeignKey_ShouldTriggerCascadeDelete()
    {
        // Arrange
        _foreignKey.OnDelete = ForeignKeyAction.Cascade;
        var row = new Row();
        var referencedRow = new Row();
        _mockReferenceTable.Setup(x => x.LookupReferencedRow(row)).Returns(referencedRow);
        
        // Act
        var result = _foreignKey.Validate(row);
        
        // Assert
        result.Should().BeTrue();
        _mockRecordManager.Verify(x => x.CascadeAction(It.IsAny<List<Row>>()), Times.Once);
    }

    [Fact]
    public void ForeignKey_ShouldTriggerCascadeUpdate()
    {
        // Arrange
        _foreignKey.OnUpdate = ForeignKeyAction.Cascade;
        var row = new Row();
        var referencedRow = new Row();
        _mockReferenceTable.Setup(x => x.LookupReferencedRow(row)).Returns(referencedRow);
        
        // Act
        var result = _foreignKey.Validate(row);
        
        // Assert
        result.Should().BeTrue();
        _mockRecordManager.Verify(x => x.CascadeAction(It.IsAny<List<Row>>()), Times.Once);
    }

    [Fact]
    public void ForeignKey_ShouldTriggerSetNullOnDelete()
    {
        // Arrange
        _foreignKey.OnDelete = ForeignKeyAction.SetNull;
        var row = new Row();
        var referencedRow = new Row();
        _mockReferenceTable.Setup(x => x.LookupReferencedRow(row)).Returns(referencedRow);
        
        // Act
        var result = _foreignKey.Validate(row);
        
        // Assert
        result.Should().BeTrue();
        _mockRecordManager.Verify(x => x.CascadeAction(It.IsAny<List<Row>>()), Times.Once);
    }
}
