using System;

namespace DBMS.Domain.Foundation;

public struct ColumnType
{
    public DbType DataType { get; }
    public int MaxLength { get; }
    public bool IsNullable { get; }
    public bool IsFixedLength { get; }

    public ColumnType(DbType dataType, int maxLength = 0, bool isNullable = false, bool isFixedLength = true)
    {
        throw new NotImplementedException("ColumnType logic not implemented yet.");
    }
}
