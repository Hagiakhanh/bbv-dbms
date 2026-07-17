using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class ForeignKey
{
    public Table ReferenceTable { get; set; }
    public List<Column> ReferenceColumns { get; set; }
}
