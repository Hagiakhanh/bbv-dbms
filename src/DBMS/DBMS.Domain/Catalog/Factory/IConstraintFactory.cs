using DBMS.Domain.Core;

namespace DBMS.Domain.Catalog.Factory;

public interface IConstraintFactory
{
    Constraint Create(ConstraintType type, ConstraintOptions options);
}

