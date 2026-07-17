namespace DBMS.Domain.Models;
using System;

public class RID : IEquatable<RID>
{
    public PageId PageId { get; }
    public int SlotNumber { get; }

    public RID(PageId pageId, int slotNumber)
    {
        PageId = pageId;
        SlotNumber = slotNumber;
    }

    public bool Equals(RID? other)

    {

        throw new NotImplementedException();

    }
}
