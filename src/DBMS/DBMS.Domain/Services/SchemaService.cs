using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Builder;
using DBMS.Domain.Catalog.Strategy;
using DBMS.Domain.Storage;

namespace DBMS.Domain.Services;

public class SchemaService : ISchemaService
{
    private readonly ICatalogManager _catalog;
    private readonly StorageEngine _storage;
    private readonly TableDirector _tableDirector;
    private readonly ITableBuilder _builder;
    private readonly IConstraintFactory _constraintFactory;
    private readonly IIndexFactory _indexFactory;

    public SchemaService(
        ICatalogManager catalog,
        StorageEngine storage,
        TableDirector tableDirector,
        ITableBuilder builder,
        IConstraintFactory constraintFactory,
        IIndexFactory indexFactory)
    {
        _catalog = catalog;
        _storage = storage;
        _tableDirector = tableDirector;
        _builder = builder;
        _constraintFactory = constraintFactory;
        _indexFactory = indexFactory;
    }

    public Table CreateTable(Schema schema, TableDefinition definition)
    {
        throw new NotImplementedException();
    }

    public void DropTable(Schema schema, string name, bool cascade)
    {
        throw new NotImplementedException();
    }

    public void RenameTable(Schema schema, string oldName, string newName)
    {
        throw new NotImplementedException();
    }

    public void AddColumn(Table table, Column column)
    {
        throw new NotImplementedException();
    }

    public void DropColumn(Table table, string name)
    {
        throw new NotImplementedException();
    }

    public void AddConstraint(Table table, Constraint constraint)
    {
        throw new NotImplementedException();
    }

    public void DropConstraint(Table table, string name)
    {
        throw new NotImplementedException();
    }

    public View CreateView(Schema schema, string name, string query)
    {
        throw new NotImplementedException();
    }

    public void DropView(Schema schema, string name)
    {
        throw new NotImplementedException();
    }

    public StoredProcedure CreateProcedure(Schema schema, string name, string body)
    {
        throw new NotImplementedException();
    }

    public void DropProcedure(Schema schema, string name)
    {
        throw new NotImplementedException();
    }

    public Sequence CreateSequence(Schema schema, string name)
    {
        throw new NotImplementedException();
    }

    public void DropSequence(Schema schema, string name)
    {
        throw new NotImplementedException();
    }
}
