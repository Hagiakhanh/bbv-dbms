using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Row
{
    public RID RowId { get; set; }
    public RecordData Data { get; set; }
    public long Version { get; set; }
}
