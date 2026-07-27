using System;

namespace DBMS.Domain.Storage;

public class FileManager : IFileManager, IPageStore
{
    private string _dataDir = string.Empty;

    public FileManager()
    {
    }

    public FileManager(string dataDir)
    {
        _dataDir = dataDir;
    }

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

    public int AllocatePage(int tableId)
    {
        throw new NotImplementedException();
    }

    public Page FetchPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void FlushPage(int pageId)
    {
        throw new NotImplementedException();
    }

    public void CreateDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public void DeleteDirectory(string path, bool recursive)
    {
        throw new NotImplementedException();
    }

    public bool DirectoryExists(string path)
    {
        throw new NotImplementedException();
    }

    public void WriteToFile(string path, byte[] data)
    {
        throw new NotImplementedException();
    }

    public byte[] ReadFromFile(string path)
    {
        throw new NotImplementedException();
    }

    public bool ValidateFilesExist(string path)
    {
        throw new NotImplementedException();
    }
}
