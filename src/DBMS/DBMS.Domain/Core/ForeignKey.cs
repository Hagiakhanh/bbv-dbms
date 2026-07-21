using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public enum ForeignKeyAction
{
    NoAction,
    Cascade,
    SetNull
}

public class ForeignKey : Constraint
{
    public Table ReferenceTable { get; set; }
    public List<Column> ReferenceColumns { get; set; } = new();

    public ForeignKeyAction OnDelete { get; set; } = ForeignKeyAction.NoAction;
    public ForeignKeyAction OnUpdate { get; set; } = ForeignKeyAction.NoAction;

    public IRecordManager RecordManager { get; set; }

    public ForeignKey() { }

    public ForeignKey(string name, Table referenceTable, List<Column> referenceColumns, IRecordManager recordManager)
    {
        Name = name;
        ReferenceTable = referenceTable ?? throw new ArgumentNullException(nameof(referenceTable));
        ReferenceColumns = referenceColumns ?? new List<Column>();
        RecordManager = recordManager ?? throw new ArgumentNullException(nameof(recordManager));
    }

    public override bool Validate(Row row)
    {
        throw new NotImplementedException();
    }
}
