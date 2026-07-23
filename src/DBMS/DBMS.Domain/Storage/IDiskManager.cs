namespace DBMS.Domain.Storage;

public interface IDiskManager
{
    void Initialize(object configuration);
    void Shutdown();
}
