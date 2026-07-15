using System;

namespace DBMS.Domain.DataFile;

public class DiskManager : IDiskManager
{
    public int CreateFile(string path)
    {
        throw new NotImplementedException();
    }

    public int OpenFile(string path)
    {
        throw new NotImplementedException();
    }

    public void CloseFile(int fd)
    {
        throw new NotImplementedException();
    }

    public void ReadPage(int fd, int pageId, byte[] buf)
    {
        throw new NotImplementedException();
    }

    public void WritePage(int fd, int pageId, byte[] buf)
    {
        throw new NotImplementedException();
    }
}
