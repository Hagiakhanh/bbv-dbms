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
    
    private CatalogObject LoadFromStorage(int id)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public TableMeta? GetTableMeta(string name)
    {
        throw new System.NotImplementedException();
    }
    public IndexMeta? GetIndexMeta(int id)
    {
        throw new System.NotImplementedException();
    }
    public void UpdateMeta(CatalogObject obj)
    {
        throw new System.NotImplementedException();
    }
    public void DeleteMeta(int id)
    {
        throw new System.NotImplementedException();
    }
}
