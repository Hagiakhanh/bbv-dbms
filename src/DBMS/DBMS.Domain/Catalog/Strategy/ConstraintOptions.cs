using System.Collections.Generic;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Strategy;

public enum ConstraintType
{
    PRIMARY_KEY,
    UNIQUE,
    FOREIGN_KEY,
    CHECK
}

public class ConstraintOptions
{
    public List<Column> Columns { get; set; } = new List<Column>();
    public Table ReferenceTable { get; set; }
    public List<Column> ReferenceColumns { get; set; } = new List<Column>();
    public string Expression { get; set; } = string.Empty;
}
