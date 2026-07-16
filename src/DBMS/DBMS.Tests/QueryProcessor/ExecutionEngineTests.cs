using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.QueryProcessor;

public class ExecutionEngineTests
{
    private readonly Mock<IAccessMethod> _mockScanner;
    private readonly Mock<IAuthorizationManager> _mockAuthz;
    private readonly ExecutionEngine _engine;

    public ExecutionEngineTests()
    {
        _mockScanner = new Mock<IAccessMethod>();
        _mockAuthz = new Mock<IAuthorizationManager>();
        _engine = new ExecutionEngine(_mockScanner.Object, _mockAuthz.Object);
    }

    [Fact]
    public void Execute_ShouldReturnResultCursor()
    {
        // Arrange
        var plan = new ExecutionPlan();
        var ctx = new TransactionContext();

        // Act
        var act = () => _engine.Execute(plan, ctx);

        // Assert
        act.Should().NotThrow();
    }
}
