using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public abstract class Index
{
    // <<abstract>>
    public int IndexId { get; set; }
    public string Name { get; set; }
    public List<Column> Columns { get; set; }
    public RID Search(key : object) 
    { 
        throw new NotImplementedException(); 
    }

    public void InsertKey(key : object, rid : RID) 
    { 
        throw new NotImplementedException(); 
    }
    
    public void DeleteKey(key : object) 
    { 
        throw new NotImplementedException(); 
    }
}
