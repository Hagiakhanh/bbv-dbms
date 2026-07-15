using System;

namespace DBMS.Domain.Foundation;

public struct RID : IEquatable<RID>
{
    public int PageId { get; }
    public int SlotId { get; }

    public RID(int pageId, int slotId)
    {
        throw new NotImplementedException("Constructor logic is not implemented yet.");
    }

    public override bool Equals(object? obj)
    {
        throw new NotImplementedException("Equals is not implemented yet.");
    }

    public bool Equals(RID other)
    {
        throw new NotImplementedException("Equals(RID) is not implemented yet.");
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException("GetHashCode is not implemented yet.");
    }

    public override string ToString()
    {
        throw new NotImplementedException("ToString is not implemented yet.");
    }

    public static bool operator ==(RID left, RID right)
    {
        throw new NotImplementedException("Operator == is not implemented yet.");
    }

    public static bool operator !=(RID left, RID right)
    {
        throw new NotImplementedException("Operator != is not implemented yet.");
    }
}
