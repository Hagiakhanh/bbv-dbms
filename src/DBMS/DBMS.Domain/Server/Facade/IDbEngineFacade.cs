namespace DBMS.Domain.Server.Facade;

public interface IDbEngineFacade
{
    void Start(bool safeMode);
    void Stop(bool force);
    void Restart();
    void Recover();
}
