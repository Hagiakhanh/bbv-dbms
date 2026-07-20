namespace DBMS.Domain.Storage;

public interface IWALManager
{
    void ReplayWAL();
}
