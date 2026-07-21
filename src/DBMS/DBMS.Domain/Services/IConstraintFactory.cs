using DBMS.Domain.Core;

namespace DBMS.Domain.Services;

public interface IConstraintFactory
{
    Constraint Create(ConstraintType type, ConstraintOptions options);
}
