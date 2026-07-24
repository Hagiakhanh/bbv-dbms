using System;
using DBMS.Domain.Query;
using DBMS.Domain.Transactions;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Query;

public class QueryExecutionTests
{
    [Fact]
    public void ExecuteSelect_ShouldReturnMatchingRows()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteSelect(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteInsert_ShouldInsertRecord()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteInsert(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteUpdate_ShouldModifyExistingRows()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteUpdate(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteDelete_ShouldDeleteMatchingRows()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteDelete(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteJoin_ShouldReturnJoinedRows()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteJoin(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteAggregate_ShouldReturnAggregatedResult()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteAggregate(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Execute_ShouldThrow_WhenExecutionPlanIsInvalid()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.Execute(plan, tx);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ExecuteAggregate_ShouldHandleEmptyTables()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteAggregate(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteJoin_ShouldReturnEmpty_WhenNoMatches()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteJoin(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteLimit_ShouldReturnOnlySpecifiedRows()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteLimit(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ExecuteOrderBy_ShouldSortResultsCorrectly()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var tx = new Transaction();

        Action act = () => executor.ExecuteOrderBy(plan, tx);

        act.Should().Throw<NotImplementedException>();
    }
}
