namespace DBMS.Domain.Server;

public interface IDbEngineFacade
{
    void Start(bool safeMode);
    void Stop(bool force);
    void Restart();
    void Recover();
}
