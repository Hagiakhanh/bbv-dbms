using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.QueryProcessor;

public class QueryOptimizerTests
{
    private readonly Mock<IMetadataCatalog> _mockCatalog;
    private readonly QueryOptimizer _optimizer;

    public QueryOptimizerTests()
    {
        _mockCatalog = new Mock<IMetadataCatalog>();
        var costModel = new CostModel();
        _optimizer = new QueryOptimizer(costModel, _mockCatalog.Object);
    }

    [Fact]
    public void Optimize_ShouldReturnExecutionPlan()
    {
        // Arrange
        var ast = new ValidatedAst();

        // Act
        var act = () => _optimizer.Optimize(ast);

        // Assert
        act.Should().NotThrow();
    }
}
