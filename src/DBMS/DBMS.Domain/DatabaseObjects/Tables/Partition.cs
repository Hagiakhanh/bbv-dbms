using System;
using System.Collections.Generic;

namespace DBMS.Domain.DatabaseObjects.Tables;

public class Partition : ICatalogComponent
{
    public string Name { get; private set; }
    public string PartitionKey { get; set; }
    public string PartitionType { get; set; }
}


