namespace DBMS.Domain.Security;

public interface ISecurityManager
{
    bool CheckPermission(string user, int obj, string action);
    void GrantOwnership(string dbName, string owner);
}
