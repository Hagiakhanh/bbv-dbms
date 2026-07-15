using System;

namespace DBMS.Domain.DataFile;

public interface IDiskManager
{
    int CreateFile(string path);
    int OpenFile(string path);
    void CloseFile(int fd);
    void ReadPage(int fd, int pageId, byte[] buf);
    void WritePage(int fd, int pageId, byte[] buf);
}
