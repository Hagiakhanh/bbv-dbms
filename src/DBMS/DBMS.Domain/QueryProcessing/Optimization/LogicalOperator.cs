using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Optimization;

public enum LogicalOperatorType
{
    Scan,
    Filter,
    Project,
    Join
}

public abstract class LogicalOperator
{
    public LogicalOperatorType OperatorType { get; protected set; }
    public IReadOnlyList<LogicalOperator> Children { get; protected set; } = Array.Empty<LogicalOperator>();

    public virtual void ReplaceChild(LogicalOperator oldChild, LogicalOperator newChild)
    {
        throw new NotImplementedException();
    }
}

public class LogicalScan : LogicalOperator
{
    public int TableId { get; set; }
    public string Alias { get; set; } = string.Empty;

    public LogicalScan()
    {
        OperatorType = LogicalOperatorType.Scan;
    }
}

public class LogicalFilter : LogicalOperator
{
    public object? Predicate { get; set; }
    public LogicalOperator? Child { get; set; }

    public LogicalFilter()
    {
        OperatorType = LogicalOperatorType.Filter;
    }
}

public class LogicalProject : LogicalOperator
{
    public IReadOnlyList<object> Expressions { get; set; } = Array.Empty<object>();
    public LogicalOperator? Child { get; set; }

    public LogicalProject()
    {
        OperatorType = LogicalOperatorType.Project;
    }
}

public class LogicalJoin : LogicalOperator
{
    public string JoinType { get; set; } = "Inner";
    public object? Condition { get; set; }
    public LogicalOperator? Left { get; set; }
    public LogicalOperator? Right { get; set; }

    public LogicalJoin()
    {
        OperatorType = LogicalOperatorType.Join;
    }
}
