namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IAuthorizationManager
{
    void CheckPermission(string user, int obj, string action);
    void GrantRole(string user, string role);
    void RevokeRole(string user, string role);
}
