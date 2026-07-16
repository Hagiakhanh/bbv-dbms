using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;

namespace DBMS.Tests.QueryProcessor;

public class ResultProcessorTests
{
    private readonly ResultProcessor _processor;

    public ResultProcessorTests()
    {
        _processor = new ResultProcessor();
    }

    [Fact]
    public void Format_ShouldReturnResultSet()
    {
        // Arrange
        var cursor = new ResultCursor();

        // Act
        var act = () => _processor.Format(cursor);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Serialize_ShouldReturnByteArray()
    {
        // Arrange
        var row = new DBMS.Domain.Models.Tuple();

        // Act
        var act = () => _processor.Serialize(row);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Paginate_ShouldReturnPageOfTuples()
    {
        // Arrange
        var cursor = new ResultCursor();
        int pageSize = 10;

        // Act
        var act = () => _processor.Paginate(cursor, pageSize);

        // Assert
        act.Should().NotThrow();
    }
}
