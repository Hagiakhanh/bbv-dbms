using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class View
{
    public int ViewId { get; set; }
    public string Name { get; set; }
    public string QueryDefinition { get; set; }

    public object Compile()
    {
        throw new NotImplementedException();
    }
}
