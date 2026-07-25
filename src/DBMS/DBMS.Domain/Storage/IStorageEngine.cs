namespace DBMS.Domain.Storage;

public interface IStorageEngine
{
    void InitializeStorageEngine(string dbName);
    void AllocateDatabase(string name);
    void DeallocateDatabase(string name);
    byte[] ReadPage(int pageId);
    void WritePage(int pageId, byte[] data);
    int AllocatePage(int tableId);
}
