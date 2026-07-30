using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Models;

public enum OperatorType
{
    Scan,
    IndexScan,
    Filter,
    Project,
    Join,
    Aggregate,
    Sort,
    Limit
}

public abstract class Operator
{
    public OperatorType OperatorType { get; set; }
    public IReadOnlyList<Operator> Children { get; set; } = Array.Empty<Operator>();
}
