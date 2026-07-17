namespace DBMS.Domain.DatabaseObjectManagement;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.StorageEngine;

public class SchemaManager
{
    private IMetadataCatalog catalog;

    public SchemaManager(IMetadataCatalog catalog)
    {
        this.catalog = catalog;
    }
    
    public void CreateSchema(string dbName, string schemaName)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void DropSchema(string dbName, string schemaName)
    {
        throw new System.NotImplementedException();
    }
}
