using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Trigger
{
    public string Name { get; set; }
    public string Event { get; set; }
    public string Body { get; set; }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}
