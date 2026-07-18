using System;
using System.Collections.Generic;

namespace DBMS.Domain.Storage;

public class FileManager
{
    private string dataDir;

    public byte[] Read(int pageId)
    {
        throw new NotImplementedException();
    }

    public void Write(int pageId, byte[] data)
    {
        throw new NotImplementedException();
    }

    public int AllocateFile(string path)
    {
        throw new NotImplementedException();
    }
}
