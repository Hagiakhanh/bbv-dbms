using System;

namespace DBMS.Domain.Core;

public interface IExpressionEvaluator
{
    bool Evaluate(string expression, Row row);
    bool ValidateExpression(string expression);
}
