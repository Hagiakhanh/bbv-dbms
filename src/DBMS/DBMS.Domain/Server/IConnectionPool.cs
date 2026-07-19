namespace DBMS.Domain.Server;

public interface IConnectionPool
{
    bool HasActiveConnections(string databaseName);
    void ForceCloseConnections(string databaseName);
}
