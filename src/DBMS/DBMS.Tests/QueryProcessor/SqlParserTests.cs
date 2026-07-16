using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.QueryProcessor;

namespace DBMS.Tests.QueryProcessor;

public class SqlParserTests
{
    private readonly SqlParser _parser;

    public SqlParserTests()
    {
        _parser = new SqlParser();
    }

    [Fact]
    public void Parse_ShouldReturnASTNode()
    {
        // Arrange
        var sql = "SELECT * FROM users";

        // Act
        var act = () => _parser.Parse(sql);

        // Assert
        act.Should().NotThrow();
    }
}
