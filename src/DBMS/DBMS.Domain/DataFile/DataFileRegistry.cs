using System;
using DBMS.Domain.Foundation;

namespace DBMS.Domain.DataFile;

public class DataFileRegistry : IDataFileRegistry
{
    public void RegisterFile(int fileId, string path, FileType type)
    {
        throw new NotImplementedException();
    }

    public int RegisterFile(string path, FileType type)
    {
        throw new NotImplementedException();
    }

    public string GetFilePath(int fileId)
    {
        throw new NotImplementedException();
    }

    public void UnregisterFile(int fileId)
    {
        throw new NotImplementedException();
    }

    public bool IsRegistered(int fileId)
    {
        throw new NotImplementedException();
    }
}
