namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class DiskManager
{
    private Dictionary<int, FileHandle> openFiles;
    private string dataDir;
    private FileHandle EnsureOpen(string path)
    {
        throw new System.NotImplementedException();
    }
    public byte[] ReadPage(PageId id)
    {
        throw new System.NotImplementedException();
    }
    public void WritePage(PageId id, byte[] data)
    {
        throw new System.NotImplementedException();
    }
    public int AllocateFile(string path)
    {
        throw new System.NotImplementedException();
    }
}
