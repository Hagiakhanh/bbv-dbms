using System;
using System.Collections.Generic;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Core;

public class UniqueConstraint : Constraint
{
    public List<Column> Columns { get; set; } = new();
    public Index Index { get; set; }
    public IRowKeyExtractor Extractor { get; set; }

    public UniqueConstraint() { }

    public UniqueConstraint(string name, List<Column> columns, Index index, IRowKeyExtractor extractor)
    {
        Name = name;
        Columns = columns ?? new List<Column>();
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
    }

    public override bool Validate(Row row)
    {
        if (row == null) throw new ArgumentNullException(nameof(row));

        if (Extractor.HasNullValue(row, Columns))
        {
            return true;
        }

        var key = Extractor.ExtractKey(row, Columns);
        var existingRid = Index.Search(key);

        if (existingRid != null)
        {
            throw new UniqueConstraintViolationException($"Duplicate key found for unique constraint '{Name}'.");
        }

        return true;
    }
}
