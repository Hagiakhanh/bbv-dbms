using System.Collections.Generic;

namespace DBMS.Domain.DatabaseObjects.Indexes;

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
