using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public abstract class Constraint
{
    // <<abstract>>
    public string Name { get; set; }
    public bool Validate(row : Row) 
    { 
        throw new NotImplementedException(); 
    }
}
