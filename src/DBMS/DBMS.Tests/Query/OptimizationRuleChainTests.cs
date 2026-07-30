using System;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Query;

public class OptimizationRuleChainTests
{
    [Fact]
    public void LogicalOperator_Scan_ShouldInitializeWithCorrectType()
    {
        var scan = new LogicalScan { TableId = 1, Alias = "u" };

        scan.OperatorType.Should().Be(LogicalOperatorType.Scan);
        scan.TableId.Should().Be(1);
        scan.Alias.Should().Be("u");
    }

    [Fact]
    public void LogicalOperator_Filter_ShouldInitializeWithCorrectType()
    {
        var filter = new LogicalFilter();

        filter.OperatorType.Should().Be(LogicalOperatorType.Filter);
        filter.Predicate.Should().BeNull();
        filter.Child.Should().BeNull();
    }

    [Fact]
    public void LogicalOperator_Project_ShouldInitializeWithCorrectType()
    {
        var project = new LogicalProject();

        project.OperatorType.Should().Be(LogicalOperatorType.Project);
        project.Expressions.Should().BeEmpty();
    }

    [Fact]
    public void LogicalOperator_Join_ShouldInitializeWithCorrectType()
    {
        var join = new LogicalJoin();

        join.OperatorType.Should().Be(LogicalOperatorType.Join);
        join.JoinType.Should().Be("Inner");
    }

    [Fact]
    public void OptimizationRuleBase_SetNext_ShouldChainRulesFluently()
    {
        var rule1 = new ConstantFoldingRule();
        var rule2 = new PredicatePushdownRule();

        var returned = rule1.SetNext(rule2);

        returned.Should().Be(rule2);
    }

    [Fact]
    public void OptimizationRulePipeline_AddRuleAndBuildChain_ShouldBuildChainWithoutError()
    {
        var pipeline = new OptimizationRulePipeline();
        var rule1 = new ConstantFoldingRule();
        var rule2 = new PredicatePushdownRule();

        pipeline.AddRule(rule1).AddRule(rule2);
        pipeline.BuildChain();

        pipeline.Should().NotBeNull();
    }

    [Fact]
    public void OptimizationRulePipeline_OptimizeUntilStable_ShouldThrowNotImplementedException()
    {
        var pipeline = new OptimizationRulePipeline();
        var plan = new LogicalPlan();
        var ctx = new OptimizationContext();

        Action act = () => pipeline.OptimizeUntilStable(plan, ctx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ConstantFoldingRule_Handle_ShouldThrowNotImplementedException()
    {
        var rule = new ConstantFoldingRule();
        var plan = new LogicalPlan();
        var ctx = new OptimizationContext();

        Action act = () => rule.Handle(plan, ctx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void PhysicalPlanGenerator_Generate_ShouldThrowNotImplementedException()
    {
        var generator = new PhysicalPlanGenerator();
        var plan = new LogicalPlan();
        var ctx = new OptimizationContext();

        Action act = () => generator.Generate(plan, ctx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void LogicalPlan_Clone_ShouldThrowNotImplementedException()
    {
        var plan = new LogicalPlan();

        Action act = () => plan.Clone();

        act.Should().Throw<NotImplementedException>();
    }
}
