using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Table
{
    public int TableId { get; set; }
    public string Name { get; set; }
    public List<Column> Columns { get; set; }
    public List<Constraint> Constraints { get; set; }
    public List<Index> Indexes { get; set; }
    public List<Partition> Partitions { get; set; }
    public List<Trigger> Triggers { get; set; }
}
