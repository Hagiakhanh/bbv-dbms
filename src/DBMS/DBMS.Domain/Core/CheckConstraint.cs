using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class CheckConstraint : Constraint
{
    public string Expression { get; set; }
    public IExpressionEvaluator Evaluator { get; set; }

    public CheckConstraint() { }

    public CheckConstraint(string name, string expression, IExpressionEvaluator evaluator)
    {
        Name = name;
        Expression = expression;
        Evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }

    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
