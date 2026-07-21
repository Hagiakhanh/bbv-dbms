using System.Collections.Generic;

namespace DBMS.Domain.Core;

public enum IndexType
{
    BTREE,
    HASH,
    BITMAP
}

public class IndexOptions
{
    public string Name { get; set; }
    public List<Column> Columns { get; set; } = new();
    public bool Unique { get; set; }
}
