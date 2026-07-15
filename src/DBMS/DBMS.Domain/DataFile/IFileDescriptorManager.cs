using System;

namespace DBMS.Domain.DataFile;

public interface IFileDescriptorManager
{
    int GetDescriptor(int fileId);
    void ReleaseDescriptor(int fileId);
    int GetActiveCount();
}
