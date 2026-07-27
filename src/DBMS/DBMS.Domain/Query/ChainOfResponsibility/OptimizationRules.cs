using System;

namespace DBMS.Domain.Query.ChainOfResponsibility;

public class ConstantFoldingRule : OptimizationRuleBase
{
    protected override bool CanApply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected override OptimizationResult Apply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }
}

public class PredicatePushdownRule : OptimizationRuleBase
{
    protected override bool CanApply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected override OptimizationResult Apply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }
}

public class ProjectionPruningRule : OptimizationRuleBase
{
    protected override bool CanApply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected override OptimizationResult Apply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }
}

public class JoinReorderingRule : OptimizationRuleBase
{
    protected override bool CanApply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected override OptimizationResult Apply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }
}
