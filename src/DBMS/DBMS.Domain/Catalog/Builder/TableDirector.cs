using System;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Factory;

namespace DBMS.Domain.Catalog.Builder;

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
