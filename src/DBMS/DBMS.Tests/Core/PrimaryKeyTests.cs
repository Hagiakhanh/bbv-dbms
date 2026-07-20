using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Core;

public class PrimaryKeyTests
{
    private readonly Mock<DBMS.Domain.Core.Index> _mockIndex;
    private readonly Mock<IRowKeyExtractor> _mockExtractor;
    private readonly List<Column> _columns;
    private readonly PrimaryKey _primaryKey;

    public PrimaryKeyTests()
    {
        _mockIndex = new Mock<DBMS.Domain.Core.Index>();
        _mockExtractor = new Mock<IRowKeyExtractor>();
        _columns = new List<Column> { new Column { Name = "Id" } };

        _primaryKey = new PrimaryKey("PK_Test", _columns, _mockIndex.Object, _mockExtractor.Object);
    }

    [Fact]
    public void PrimaryKey_ShouldRejectNullValues()
    {
        // Arrange
        var row = new Row();
        _mockExtractor.Setup(x => x.HasNullValue(row, _columns)).Returns(true);

        // Act
        Action act = () => _primaryKey.Validate(row);

        // Assert
        act.Should().Throw<UniqueConstraintViolationException>()
            .WithMessage("*cannot be null*");
    }

    [Fact]
    public void PrimaryKey_ShouldRejectDuplicateValues()
    {
        // Arrange
        var row = new Row();
        var key = new object();
        _mockExtractor.Setup(x => x.HasNullValue(row, _columns)).Returns(false);
        _mockExtractor.Setup(x => x.ExtractKey(row, _columns)).Returns(key);
        _mockIndex.Setup(x => x.Search(key)).Returns(new RID { PageId = 1, SlotNumber = 1 });

        // Act
        Action act = () => _primaryKey.Validate(row);

        // Assert
        act.Should().Throw<UniqueConstraintViolationException>()
            .WithMessage("*Duplicate key found*");
    }

    [Fact]
    public void PrimaryKey_ShouldAcceptUniqueValues()
    {
        // Arrange
        var row = new Row();
        var key = new object();
        _mockExtractor.Setup(x => x.HasNullValue(row, _columns)).Returns(false);
        _mockExtractor.Setup(x => x.ExtractKey(row, _columns)).Returns(key);
        _mockIndex.Setup(x => x.Search(key)).Returns((RID)null);

        // Act
        var result = _primaryKey.Validate(row);

        // Assert
        result.Should().BeTrue();
    }
}
