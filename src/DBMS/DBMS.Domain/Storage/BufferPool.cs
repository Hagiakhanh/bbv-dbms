using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class BufferPool
{
    private Page[] frames;
    private string policy;

    public Page FetchPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void UnpinPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void FlushPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void MarkDirty(int pageId)
    {
        throw new NotImplementedException();
    }
}
