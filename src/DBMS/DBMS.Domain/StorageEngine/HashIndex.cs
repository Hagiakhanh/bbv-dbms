namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class HashIndex : IIndexStructure
{
    private PageId directoryPageId;
    private IBufferPool bufferPool;
    private PageId ResolveChain(int hash)
    {
        throw new System.NotImplementedException();
    }
    public void Insert(Key key, RID rid)
    {
        throw new System.NotImplementedException();
    }
    public void Delete(Key key)
    {
        throw new System.NotImplementedException();
    }
    public RID? Search(Key key)
    {
        throw new System.NotImplementedException();
    }
    public IEnumerable<RID> RangeScan(Key fromKey, Key toKey)
    {
        throw new System.NotImplementedException();
    }
}
