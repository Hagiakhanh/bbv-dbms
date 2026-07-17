using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Database
{
    public int DatabaseId { get; set; }
    public string Name { get; set; }
    public string Owner { get; set; }
    public List<Schema> Schemas { get; set; }
}
