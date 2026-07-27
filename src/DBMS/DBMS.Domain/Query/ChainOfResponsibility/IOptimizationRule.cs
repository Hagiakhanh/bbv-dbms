using System;

namespace DBMS.Domain.Query.ChainOfResponsibility;

public interface IOptimizationRule
{
    IOptimizationRule SetNext(IOptimizationRule next);
    OptimizationResult Handle(LogicalPlan plan, OptimizationContext ctx);
}
