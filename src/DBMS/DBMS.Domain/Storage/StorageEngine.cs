using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class StorageEngine
{
    public byte[] ReadPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void WritePage(int pageId, byte[] data)
    {
        throw new NotImplementedException();
    }

    public int AllocatePage(int tableId)
    {
        throw new NotImplementedException();
    }
}
