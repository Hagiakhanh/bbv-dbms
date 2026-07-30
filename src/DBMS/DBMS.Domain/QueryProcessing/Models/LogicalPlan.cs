using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Models;

public class LogicalPlan
{
    public List<Operator> Operators { get; set; } = new();
    public LogicalOperator? Root { get; set; }

    public LogicalPlan Clone()
    {
        throw new NotImplementedException();
    }
}
