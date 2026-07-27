using System;
using System.Collections.Generic;
using DBMS.Domain.Query.ChainOfResponsibility;

namespace DBMS.Domain.Query;

public class LogicalPlan
{
    public List<Operator> Operators { get; set; } = new();
    public LogicalOperator? Root { get; set; }

    public LogicalPlan Clone()
    {
        throw new NotImplementedException();
    }
}
