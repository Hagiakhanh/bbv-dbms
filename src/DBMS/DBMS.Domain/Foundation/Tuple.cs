using System;

namespace DBMS.Domain.Foundation;

public class Tuple
{
    public int Size => throw new NotImplementedException("Size is not implemented yet.");
    
    public RID Rid
    {
        get => throw new NotImplementedException("Rid getter is not implemented yet.");
        set => throw new NotImplementedException("Rid setter is not implemented yet.");
    }

    public Tuple(byte[] data)
    {
        throw new NotImplementedException("Constructor logic is not implemented yet.");
    }

    public byte[] GetData()
    {
        throw new NotImplementedException("GetData is not implemented yet.");
    }
}
