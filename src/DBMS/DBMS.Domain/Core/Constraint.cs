using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public abstract class Constraint
{
    public string Name { get; set; }

    public abstract bool Validate(Row row);
}
