using System;
using System.Collections.Generic;

namespace DBMS.Domain.Services;

public class SchemaService
{
    private CatalogManager catalog;
    private StorageEngine storage;
    private readonly ITableBuilder _builder;
    private readonly IConstraintFactory _constraintFactory;
    private readonly IIndexFactory _indexFactory;

    public SchemaService(ITableBuilder builder, IConstraintFactory constraintFactory, IIndexFactory indexFactory)
    {
        _builder = builder;
        _constraintFactory = constraintFactory;
        _indexFactory = indexFactory;
    }

    public Table CreateTable(Schema schema, TableDef def)
    {
        _builder.Reset(def.Name);

        foreach (var col in def.Columns)
        {
            _builder.AddColumn(col);
        }

        foreach (var constraint in def.Constraints)
        {
            _builder.AddConstraint(constraint);
        }

        var table = _builder.Build();
        table.Parent = schema;
        schema.AddTable(table);

        return table;
    }

    public void DropTable(Schema schema, string name)
    {
        schema.DropTable(name);
    }

    public View CreateView(Schema schema, string name, string query)
    {
        var view = new View(name, query);
        schema.CreateView(view);
        return view;
    }

    public void DropView(Schema schema, string name)
    {
        schema.DropView(name);
    }
}
