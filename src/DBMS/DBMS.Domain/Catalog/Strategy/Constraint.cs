using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Strategy;

public abstract class Constraint : ICatalogComponent
{
    public string Name { get; set; }

    public abstract bool Validate(Row row);
}


