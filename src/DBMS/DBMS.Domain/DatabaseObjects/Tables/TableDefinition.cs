using System;
using System.Collections.Generic;

namespace DBMS.Domain.DatabaseObjects.Tables;

public class TableDefinition
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<Column> Columns { get; set; } = new List<Column>();
    public IReadOnlyCollection<ConstraintOptions> Constraints { get; set; } = new List<ConstraintOptions>();
    public IReadOnlyCollection<IndexOptions> Indexes { get; set; } = new List<IndexOptions>();
    public IReadOnlyCollection<Partition> Partitions { get; set; } = new List<Partition>();
    public IReadOnlyCollection<Trigger> Triggers { get; set; } = new List<Trigger>();
}
