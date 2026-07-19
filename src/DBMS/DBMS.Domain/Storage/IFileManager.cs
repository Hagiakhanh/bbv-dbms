namespace DBMS.Domain.Storage;

public interface IFileManager
{
    byte[] Read(int pageId);
    void Write(int pageId, byte[] data);
    int AllocateFile(string path);
    
    // Additional methods for DB Management
    void CreateDirectory(string path);
    void DeleteDirectory(string path, bool recursive);
    bool DirectoryExists(string path);
    void WriteToFile(string path, byte[] data);
    byte[] ReadFromFile(string path);
}
