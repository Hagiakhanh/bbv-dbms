using System;

namespace DBMS.Domain.QueryProcessing.Optimization;

public abstract class OptimizationRuleBase : IOptimizationRule
{
    private IOptimizationRule? _next;

    public IOptimizationRule SetNext(IOptimizationRule next)
    {
        _next = next;
        return next;
    }

    public virtual OptimizationResult Handle(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected virtual bool CanApply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected virtual OptimizationResult Apply(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }

    protected virtual OptimizationResult PassToNext(OptimizationResult result, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }
}
