using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Composite;

public class Column : ICatalogComponent
{
    public int ColumnId { get; set; }
    public string Name { get; set; }
    public Table Parent { get; set; }
    public DataTypeEnum DataType { get; set; }
    public bool Nullable { get; set; }
    public object DefaultValue { get; set; }
}


