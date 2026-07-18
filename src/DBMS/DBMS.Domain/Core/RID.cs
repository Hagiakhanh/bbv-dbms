using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

// <<value object>>
public class RID
{
    public int PageId { get; set; }
    public int SlotNumber { get; set; }

    public bool Equals(RID other)
    {
        throw new NotImplementedException();
    }
}
