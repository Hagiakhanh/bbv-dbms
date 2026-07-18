using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class ForeignKey : Constraint
{
    public Table ReferenceTable { get; set; }
    public List<Column> ReferenceColumns { get; set; }

    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
