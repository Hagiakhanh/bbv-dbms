using System;
using System.Collections.Generic;

namespace DBMS.Domain.DatabaseObjects.Constraints;

public abstract class Constraint : ICatalogComponent
{
    public string Name { get; set; }

    public abstract bool Validate(Row row);
}


