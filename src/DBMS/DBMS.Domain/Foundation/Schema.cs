using System;
using System.Collections.Generic;

namespace DBMS.Domain.Foundation;

public class Schema
{
    public int ColumnCount => throw new NotImplementedException();
    
    public IReadOnlyList<string> ColumnNames => throw new NotImplementedException();
    
    public IReadOnlyList<ColumnType> ColumnTypes => throw new NotImplementedException();
    
    public bool IsFixedLength => throw new NotImplementedException();

    public Schema(IEnumerable<string> names, IEnumerable<ColumnType> types)
    {
        throw new NotImplementedException("Schema constructor not implemented yet.");
    }

    public int GetColumnIndex(string name)
    {
        throw new NotImplementedException("GetColumnIndex not implemented yet.");
    }
}
