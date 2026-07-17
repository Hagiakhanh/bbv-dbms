namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class BPlusTree : IIndexStructure
{
    private PageId rootPageId;
    private int degree;
    private IBufferPool bufferPool;
    private PageId SplitNode(PageId nodeId)
    {
        throw new System.NotImplementedException();
    }
    private void PropagateSplit(PageId child, PageId parent)
    {
        throw new System.NotImplementedException();
    }
    private PageId FindLeaf(Key key)
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
