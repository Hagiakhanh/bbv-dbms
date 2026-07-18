using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class CheckConstraint : Constraint
{
    public string Expression { get; set; }

    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
