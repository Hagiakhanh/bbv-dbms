using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class PrimaryKey : Constraint
{
    public List<Column> Columns { get; set; }

    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
