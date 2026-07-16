namespace DBMS.Domain.DatabaseObjectManagement;

using System.Collections.Generic;

public class DatabaseManager
{
    public MetadataCatalog Catalog { get; set; }

    public void CreateDatabase() => throw new System.NotImplementedException();
    public void DropDatabase() => throw new System.NotImplementedException();
}

public class SchemaManager
{
    public Models.DatabaseId DbId { get; set; }

    public void CreateSchema() => throw new System.NotImplementedException();
}

public class TableManager
{
    public Models.SchemaId SchemaId { get; set; }
    public StorageEngine.IndexManager IndexMgr { get; set; }

    public void CreateTable() => throw new System.NotImplementedException();
    public void AlterTable() => throw new System.NotImplementedException();
}

public class MetadataCatalog
{
    public Dictionary<string, object> SysTables { get; set; }

    public void GetTableMeta() => throw new System.NotImplementedException();
    public void UpdateMeta() => throw new System.NotImplementedException();
}
