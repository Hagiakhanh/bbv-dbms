namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class RecordLayoutManager
{
    private Schema schema;
    public int GetFieldOffset(int colId)
    {
        throw new System.NotImplementedException();
    }
    public byte[] Serialize(Record record)
    {
        throw new System.NotImplementedException();
    }
    public Record Deserialize(byte[] data)
    {
        throw new System.NotImplementedException();
    }
}
