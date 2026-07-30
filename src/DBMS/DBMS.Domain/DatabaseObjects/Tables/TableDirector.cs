using System;

namespace DBMS.Domain.DatabaseObjects.Tables;

public class TableDirector
{
    private readonly ITableBuilder _builder;
    private readonly IConstraintFactory _constraintFactory;
    private readonly IIndexFactory _indexFactory;

    public TableDirector(ITableBuilder builder, IConstraintFactory constraintFactory, IIndexFactory indexFactory)
    {
        _builder = builder;
        _constraintFactory = constraintFactory;
        _indexFactory = indexFactory;
    }

    public Table Construct(TableDefinition definition)
    {
        throw new NotImplementedException();
    }
}
