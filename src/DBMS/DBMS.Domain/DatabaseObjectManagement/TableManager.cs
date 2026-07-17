namespace DBMS.Domain.DatabaseObjectManagement;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.StorageEngine;

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
    
    public void CreateTable(TableDef def)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void AlterTable(string name, TablePatch patch)
    {
        throw new System.NotImplementedException();
    }
    public void DropTable(string name)
    {
        throw new System.NotImplementedException();
    }
}
