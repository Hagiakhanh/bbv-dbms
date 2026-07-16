namespace DBMS.Domain.DatabaseObjectManagement;

using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.StorageEngine;

public class MetadataCatalog : IMetadataCatalog
{
    private Dictionary<string, CatalogObject> sysTables;
    private IBufferPool bufferPool;

    public MetadataCatalog(IBufferPool bufferPool)
    {
        this.bufferPool = bufferPool;
        this.sysTables = new Dictionary<string, CatalogObject>();
    }
    
    private CatalogObject LoadFromStorage(ObjectId id) => throw new System.NotImplementedException();
    public TableMeta? GetTableMeta(TableName name) => throw new System.NotImplementedException();
    public IndexMeta? GetIndexMeta(IndexId id) => throw new System.NotImplementedException();
    public void UpdateMeta(CatalogObject obj) => throw new System.NotImplementedException();
    public void DeleteMeta(ObjectId id) => throw new System.NotImplementedException();
}

public class DatabaseManager
{
    private IMetadataCatalog catalog;
    private TablespaceManager tablespaceMgr;

    public DatabaseManager(IMetadataCatalog catalog, TablespaceManager tablespaceMgr)
    {
        this.catalog = catalog;
        this.tablespaceMgr = tablespaceMgr;
    }
    
    public void CreateDatabase(DatabaseName name) => throw new System.NotImplementedException();
    public void DropDatabase(DatabaseName name) => throw new System.NotImplementedException();
    public DatabaseMeta? GetDatabase(DatabaseName name) => throw new System.NotImplementedException();
}

public class SchemaManager
{
    private IMetadataCatalog catalog;

    public SchemaManager(IMetadataCatalog catalog)
    {
        this.catalog = catalog;
    }
    
    public void CreateSchema(DatabaseName dbName, string schemaName) => throw new System.NotImplementedException();
    public void DropSchema(DatabaseName dbName, string schemaName) => throw new System.NotImplementedException();
}

public class TableManager
{
    private IMetadataCatalog catalog;
    private IndexManager indexMgr;
    private SpaceManager spaceMgr;

    public TableManager(IMetadataCatalog catalog, IndexManager indexMgr, SpaceManager spaceMgr)
    {
        this.catalog = catalog;
        this.indexMgr = indexMgr;
        this.spaceMgr = spaceMgr;
    }
    
    public void CreateTable(TableDef def) => throw new System.NotImplementedException();
    public void AlterTable(TableName name, TablePatch patch) => throw new System.NotImplementedException();
    public void DropTable(TableName name) => throw new System.NotImplementedException();
}
