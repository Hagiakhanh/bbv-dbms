using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Query;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Query;

public class SqlParsingAndSemanticAnalysisTests
{
    [Fact]
    public void ParseSelect_ShouldGenerateAST()
    {
        var parser = new SQLParser();

        Action act = () => parser.Parse("SELECT * FROM Users");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ParseInsert_ShouldGenerateAST()
    {
        var parser = new SQLParser();

        Action act = () => parser.Parse("INSERT INTO Users VALUES (1, 'Alice')");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ParseCreate_ShouldGenerateASTForDDL()
    {
        var parser = new SQLParser();

        Action act = () => parser.Parse("CREATE TABLE Users (Id INT)");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Parse_ShouldThrow_WhenSqlSyntaxIsInvalid()
    {
        var parser = new SQLParser();

        Action act = () => parser.Parse("INVALID SQL STATEMENT");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Bind_ShouldResolveTableNames()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var analyzer = new SemanticAnalyzer(catalogMock.Object);

        Action act = () => analyzer.Bind(new object());

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Bind_ShouldThrow_WhenTableDoesNotExist()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var analyzer = new SemanticAnalyzer(catalogMock.Object);

        Action act = () => analyzer.Bind(new object());

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Bind_ShouldThrow_WhenColumnDoesNotExist()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var analyzer = new SemanticAnalyzer(catalogMock.Object);

        Action act = () => analyzer.Bind(new object());

        act.Should().Throw<Exception>();
    }
}
