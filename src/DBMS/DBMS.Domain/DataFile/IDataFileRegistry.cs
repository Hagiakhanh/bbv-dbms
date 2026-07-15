using System;
using DBMS.Domain.Foundation;

namespace DBMS.Domain.DataFile;

public interface IDataFileRegistry
{
    void RegisterFile(int fileId, string path, FileType type);
    string GetFilePath(int fileId);
    void UnregisterFile(int fileId);
    bool IsRegistered(int fileId);
    
    // Additional method inferred from tests: "RegisterFile(string path, FileType type) -> int fileId"
    int RegisterFile(string path, FileType type);
}
