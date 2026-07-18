using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Column
{
    public int ColumnId { get; set; }
    public string Name { get; set; }
    public DataTypeEnum DataType { get; set; }
    public bool Nullable { get; set; }
    public object DefaultValue { get; set; }
}
