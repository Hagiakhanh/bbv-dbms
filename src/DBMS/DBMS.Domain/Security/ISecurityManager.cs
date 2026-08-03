namespace DBMS.Domain.Security;

public interface ISecurityManager
{
    User? Authenticate(string username, string password);
    bool CheckPermission(string user, int obj, string action);
    bool HasPermission(string user, int obj, string action);
    bool Authorize(string user, int obj, string action);
    void GrantRole(string user, string role);
    void RevokeRole(string user, string role);
    void GrantOwnership(string dbName, string owner);
}
