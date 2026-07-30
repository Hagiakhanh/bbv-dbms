using System;

namespace DBMS.Domain.QueryProcessing.Optimization;

public interface IOptimizationRule
{
    IOptimizationRule SetNext(IOptimizationRule next);
    OptimizationResult Handle(LogicalPlan plan, OptimizationContext ctx);
}
