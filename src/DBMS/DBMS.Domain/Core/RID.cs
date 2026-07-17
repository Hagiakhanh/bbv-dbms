using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class RID
{
    // <<value object>>
    public int PageId { get; set; }
    public int SlotNumber { get; set; }
    public bool Equals(other : RID) 
    { 
        throw new NotImplementedException(); 
    }
}
