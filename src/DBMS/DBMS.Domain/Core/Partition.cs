using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Partition
{
    public string PartitionKey { get; set; }
    public PartitionType PartitionType { get; set; }
}
