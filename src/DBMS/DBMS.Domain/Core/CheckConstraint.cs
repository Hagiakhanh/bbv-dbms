using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class CheckConstraint : Constraint
{
    public string Expression { get; set; }
    private readonly IExpressionEvaluator _evaluator;

    public CheckConstraint(string name, string expression, IExpressionEvaluator evaluator)
    {
        Name = name;
        Expression = expression;
        _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }

    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
