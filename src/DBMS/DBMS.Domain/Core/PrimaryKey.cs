using System;
using System.Collections.Generic;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Core;

public class PrimaryKey : Constraint
{
    public List<Column> Columns { get; set; } = new();
    public Index Index { get; set; }
    public IRowKeyExtractor Extractor { get; set; }

    public PrimaryKey() { }

    public PrimaryKey(string name, List<Column> columns, Index index, IRowKeyExtractor extractor)
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
            throw new UniqueConstraintViolationException($"Primary key column cannot be null in constraint '{Name}'.");
        }

        var key = Extractor.ExtractKey(row, Columns);
        var existingRid = Index.Search(key);

        if (existingRid != null)
        {
            throw new UniqueConstraintViolationException($"Duplicate key found for primary key constraint '{Name}'.");
        }

        return true;
    }
}
