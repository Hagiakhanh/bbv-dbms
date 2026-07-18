using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class UniqueConstraint : Constraint
{
    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
