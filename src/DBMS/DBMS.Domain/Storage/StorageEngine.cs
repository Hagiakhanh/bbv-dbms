using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class StorageEngine
{
    public Byte[] ReadPage(id : PageId) 
    { 
        throw new NotImplementedException(); 
    }

    public void WritePage(id : PageId, data : Byte[]) 
    { 
        throw new NotImplementedException(); 
    }

    public PageId AllocatePage(tableId : int) 
    { 
        throw new NotImplementedException(); 
    }
}
