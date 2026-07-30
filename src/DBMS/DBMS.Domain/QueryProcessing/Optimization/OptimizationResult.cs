using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Optimization;

public class OptimizationResult
{
    public LogicalPlan Plan { get; set; } = null!;
    public bool Changed { get; set; }
    public IReadOnlyList<string> AppliedRules { get; set; } = Array.Empty<string>();
}
