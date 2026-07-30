using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Optimization;

public class OptimizationRulePipeline
{
    private IOptimizationRule? _firstRule;
    private readonly List<IOptimizationRule> _rules = new();
    private int _maxPasses = 10;

    public OptimizationRulePipeline AddRule(IOptimizationRule rule)
    {
        _rules.Add(rule);
        return this;
    }

    public void BuildChain()
    {
        if (_rules.Count == 0)
        {
            _firstRule = null;
            return;
        }

        _firstRule = _rules[0];
        IOptimizationRule current = _firstRule;

        for (int i = 1; i < _rules.Count; i++)
        {
            current = current.SetNext(_rules[i]);
        }
    }

    public LogicalPlan OptimizeUntilStable(LogicalPlan plan, OptimizationContext ctx)
    {
        throw new NotImplementedException();
    }
}
