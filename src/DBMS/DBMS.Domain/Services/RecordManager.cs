using System;
using System.Collections.Generic;

namespace DBMS.Domain.Services;

public class RecordManager
{
    private StorageEngine storage;
    private CatalogManager catalog;

    public RID Insert(Table table, Row row)
    {
        throw new NotImplementedException();
    }

    public void Update(Table table, RID rid, Row row)
    {
        throw new NotImplementedException();
    }

    public void Delete(Table table, RID rid)
    {
        throw new NotImplementedException();
    }

    public Row Read(Table table, RID rid)
    {
        throw new NotImplementedException();
    }

    public List<Row> Scan(Table table)
    {
        throw new NotImplementedException();
    }
}
