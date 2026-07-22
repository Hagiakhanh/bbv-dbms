using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Strategy;

public enum IndexType
{
    BTREE,
    HASH,
    BITMAP
}

public class IndexOptions
{
    public string Name { get; set; } = string.Empty;
    public List<Column> Columns { get; set; } = new List<Column>();
    public bool Unique { get; set; }
}
