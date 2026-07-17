using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class PrimaryKey
{
    public List<Column> Columns { get; set; }
}
