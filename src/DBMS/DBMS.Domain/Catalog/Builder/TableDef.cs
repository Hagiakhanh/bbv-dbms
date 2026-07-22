using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Builder;

/// <summary>
/// Represents the definition used when creating a new table.
/// </summary>
public class TableDef
{
    public string Name { get; set; } = string.Empty;
    public List<Column> Columns { get; set; } = new();
    public List<Constraint> Constraints { get; set; } = new();
}

