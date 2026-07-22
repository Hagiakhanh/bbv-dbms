using System;
using DBMS.Domain.Core;

namespace DBMS.Domain.Catalog.Factory;

public class ConstraintFactory : IConstraintFactory
{
    public Constraint Create(ConstraintType type, ConstraintOptions options)
    {
        return type switch
        {
            ConstraintType.PRIMARY_KEY => new PrimaryKey { Name = "PK_" + Guid.NewGuid().ToString("N") },
            ConstraintType.UNIQUE => new UniqueConstraint { Name = "UQ_" + Guid.NewGuid().ToString("N") },
            ConstraintType.FOREIGN_KEY => new ForeignKey { Name = "FK_" + Guid.NewGuid().ToString("N"), ReferenceTable = options.ReferenceTable },
            ConstraintType.CHECK => new CheckConstraint { Name = "CHK_" + Guid.NewGuid().ToString("N"), Expression = options.Expression },
            _ => throw new ArgumentException("Unknown constraint type")
        };
    }
}

