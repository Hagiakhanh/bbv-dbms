using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Column
{
    public int ColumnId { get; set; }
    public string Name { get; set; }
    public DataType DataType { get; set; }
    public bool Nullable { get; set; }
    public object DefaultValue { get; set; }
}
