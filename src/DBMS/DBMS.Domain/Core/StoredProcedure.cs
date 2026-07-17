using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class StoredProcedure
{
    public string Name { get; set; }
    public List<Column> Parameters { get; set; }
    public string Body { get; set; }
}
