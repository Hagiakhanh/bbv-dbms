using DBMS.Domain.Core;

namespace DBMS.Domain.DatabaseObjects.Constraints;

public interface IConstraintFactory
{
    Constraint Create(ConstraintType type, ConstraintOptions options);
}

