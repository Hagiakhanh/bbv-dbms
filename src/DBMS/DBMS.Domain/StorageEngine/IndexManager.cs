namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class IndexManager
{
    private IBufferPool bufferPool;
    private IMetadataCatalog catalog;
    private Dictionary<int, IIndexStructure> indexCache;
    public int CreateIndex(IndexDef def)
    {
        throw new System.NotImplementedException();
    }
    public void DropIndex(int id)
    {
        throw new System.NotImplementedException();
    }
    public IIndexStructure GetIndex(int id)
    {
        throw new System.NotImplementedException();
    }
}
