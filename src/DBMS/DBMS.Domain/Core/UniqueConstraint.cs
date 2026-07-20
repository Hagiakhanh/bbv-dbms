using System;
using System.Collections.Generic;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Core;

public class UniqueConstraint : Constraint
{
    public List<Column> Columns { get; }
    private readonly Index _index;
    private readonly IRowKeyExtractor _extractor;

    public UniqueConstraint(string name, List<Column> columns, Index index, IRowKeyExtractor extractor)
    {
        Name = name;
        Columns = columns ?? new List<Column>();
        _index = index ?? throw new ArgumentNullException(nameof(index));
        _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
    }

    public override bool Validate(Row row)
    {
        if (row == null) throw new ArgumentNullException(nameof(row));

        if (_extractor.HasNullValue(row, Columns))
        {
            return true;
        }

        var key = _extractor.ExtractKey(row, Columns);
        var existingRid = _index.Search(key);

        if (existingRid != null)
        {
            throw new UniqueConstraintViolationException($"Duplicate key found for unique constraint '{Name}'.");
        }

        return true;
    }
}
