using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Sequence
{
    public string Name { get; set; }
    public long CurrentValue { get; set; }
    public long Increment { get; set; }

    public long NextValue()
    {
        throw new NotImplementedException();
    }
}
