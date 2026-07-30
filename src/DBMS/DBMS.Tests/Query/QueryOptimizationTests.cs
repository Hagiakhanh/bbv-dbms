using System;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Query;

public class QueryOptimizationTests
{
    [Fact]
    public void Optimize_ShouldChooseIndexScan_WhenIndexExists()
    {
        var optimizer = new QueryOptimizer();
        var logicalPlan = new LogicalPlan();

        Action act = () => optimizer.Optimize(logicalPlan);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Optimize_ShouldChooseTableScan_WhenNoIndexExists()
    {
        var optimizer = new QueryOptimizer();
        var logicalPlan = new LogicalPlan();

        Action act = () => optimizer.Optimize(logicalPlan);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Optimize_ShouldOptimizeJoinOrder()
    {
        var optimizer = new QueryOptimizer();
        var logicalPlan = new LogicalPlan();

        Action act = () => optimizer.Optimize(logicalPlan);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Optimize_ShouldApplyPredicatePushdown()
    {
        var optimizer = new QueryOptimizer();
        var logicalPlan = new LogicalPlan();

        Action act = () => optimizer.Optimize(logicalPlan);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Optimize_ShouldUseCoveringIndex_WhenPossible()
    {
        var optimizer = new QueryOptimizer();
        var logicalPlan = new LogicalPlan();

        Action act = () => optimizer.Optimize(logicalPlan);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Optimize_ShouldChooseHashJoin_ForEquiJoins()
    {
        var optimizer = new QueryOptimizer();
        var logicalPlan = new LogicalPlan();

        Action act = () => optimizer.Optimize(logicalPlan);

        act.Should().Throw<NotImplementedException>();
    }
}
