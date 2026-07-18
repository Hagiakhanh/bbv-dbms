using System;
using System.Collections.Generic;

namespace DBMS.Domain.Services;

public class SchemaService
{
    private CatalogManager catalog;
    private StorageEngine storage;

    public Table CreateTable(Schema schema, TableDef def)
    {
        throw new NotImplementedException();
    }

    public void DropTable(Schema schema, string name)
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
}
