using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class FileManager
{
    private string dataDir;
    public Byte[] Read(pageId : PageId) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void Write(pageId : PageId, data : Byte[]) 
    { 
        throw new NotImplementedException(); 
    }
    
    public int AllocateFile(path : string) 
    { 
        throw new NotImplementedException(); 
    }
}
