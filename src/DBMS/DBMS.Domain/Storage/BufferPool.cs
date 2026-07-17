using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class BufferPool
{
    private Page[] frames;
    private ReplacementPolicy policy;
    public Page FetchPage(id : PageId) 
    { 
        throw new NotImplementedException(); 
    }

    public void UnpinPage(id : PageId) 
    { 
        throw new NotImplementedException(); 
    }

    public void FlushPage(id : PageId) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void MarkDirty(id : PageId) 
    { 
        throw new NotImplementedException(); 
    }
}
