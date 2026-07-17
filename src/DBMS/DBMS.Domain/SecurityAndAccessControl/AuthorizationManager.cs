namespace DBMS.Domain.SecurityAndAccessControl;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.DatabaseObjectManagement;

public class AuthorizationManager : IAuthorizationManager
{
    private IMetadataCatalog catalog;
    
    private HashSet<string> ResolveRoles(string user)
    
    {
    
        throw new System.NotImplementedException();
    
    }
    public void CheckPermission(string user, int obj, string action)
    {
        throw new System.NotImplementedException();
    }
    public void GrantRole(string user, string role)
    {
        throw new System.NotImplementedException();
    }
    public void RevokeRole(string user, string role)
    {
        throw new System.NotImplementedException();
    }
}
