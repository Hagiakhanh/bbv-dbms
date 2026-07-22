using System.Collections.Generic;

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
    public List<Column> Columns { get; set; } = new();
    public Table ReferenceTable { get; set; }
    public List<Column> ReferenceColumns { get; set; } = new();
    public string Expression { get; set; }
}

