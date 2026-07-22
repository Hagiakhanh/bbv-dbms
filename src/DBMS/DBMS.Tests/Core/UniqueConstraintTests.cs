using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Core;

public class UniqueConstraintTests
{
    private readonly Mock<DBMS.Domain.Catalog.Strategy.Index> _mockIndex;
    private readonly Mock<IRowKeyExtractor> _mockExtractor;
    private readonly List<Column> _columns;
    private readonly UniqueConstraint _uniqueConstraint;

    public UniqueConstraintTests()
    {
        _mockIndex = new Mock<DBMS.Domain.Catalog.Strategy.Index>();
        _mockExtractor = new Mock<IRowKeyExtractor>();
        _columns = new List<Column> { new Column { Name = "Email" } };

        _uniqueConstraint = new UniqueConstraint("UQ_Test", _columns, _mockIndex.Object, _mockExtractor.Object);
    }

    [Fact]
    public void UniqueConstraint_ShouldAllowNullValues()
    {
        // Arrange
        var row = new Row();
        _mockExtractor.Setup(x => x.HasNullValue(row, _columns)).Returns(true);

        // Act
        var result = _uniqueConstraint.Validate(row);

        // Assert
        result.Should().BeTrue();
        _mockIndex.Verify(x => x.Search(It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public void UniqueConstraint_ShouldRejectDuplicateValues()
    {
        // Arrange
        var row = new Row();
        var key = new object();
        _mockExtractor.Setup(x => x.HasNullValue(row, _columns)).Returns(false);
        _mockExtractor.Setup(x => x.ExtractKey(row, _columns)).Returns(key);
        _mockIndex.Setup(x => x.Search(key)).Returns(new RID { PageId = 2, SlotNumber = 1 });

        // Act
        Action act = () => _uniqueConstraint.Validate(row);

        // Assert
        act.Should().Throw<UniqueConstraintViolationException>()
            .WithMessage("*Duplicate key found*");
    }

    [Fact]
    public void UniqueConstraint_ShouldAcceptUniqueValues()
    {
        // Arrange
        var row = new Row();
        var key = new object();
        _mockExtractor.Setup(x => x.HasNullValue(row, _columns)).Returns(false);
        _mockExtractor.Setup(x => x.ExtractKey(row, _columns)).Returns(key);
        _mockIndex.Setup(x => x.Search(key)).Returns((RID)null);

        // Act
        var result = _uniqueConstraint.Validate(row);

        // Assert
        result.Should().BeTrue();
    }
}

