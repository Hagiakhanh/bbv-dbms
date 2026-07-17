using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Schema
{
    public int SchemaId { get; set; }
    public string Name { get; set; }
    public List<Table> Tables { get; set; }
    public List<View> Views { get; set; }
    public List<StoredProcedure> Procedures { get; set; }
    public List<Sequence> Sequences { get; set; }
}
