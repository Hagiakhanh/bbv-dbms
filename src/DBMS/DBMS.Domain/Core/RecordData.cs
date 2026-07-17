using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class RecordData
{
    // <<value object>>
    public Byte[] Bytes { get; set; }
    public int Length { get; set; }
}
