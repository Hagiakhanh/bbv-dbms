using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

// <<value object>>
public class RecordData
{
    public byte[] Bytes { get; set; }
    public int Length { get; set; }

    public byte[] Serialize()
    {
        throw new NotImplementedException();
    }

    public static RecordData Deserialize(byte[] data)
    {
        throw new NotImplementedException();
    }
}
