namespace DBMS.Domain.DatabaseObjectManagement;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.StorageEngine;

public class DatabaseManager
{
    private IMetadataCatalog catalog;
    private TablespaceManager tablespaceMgr;

    public DatabaseManager(IMetadataCatalog catalog, TablespaceManager tablespaceMgr)
    {
        this.catalog = catalog;
        this.tablespaceMgr = tablespaceMgr;
    }
    
    public void CreateDatabase(string name)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void DropDatabase(string name)
    {
        throw new System.NotImplementedException();
    }
    public DatabaseMeta? GetDatabase(string name)
    {
        throw new System.NotImplementedException();
    }
}
