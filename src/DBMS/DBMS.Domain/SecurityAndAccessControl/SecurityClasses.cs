namespace DBMS.Domain.SecurityAndAccessControl;

using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.DatabaseObjectManagement;

public class AuthenticationManager
{
    private Dictionary<string, HashedCredential> userDb;
    
    private HashedCredential HashPassword(string raw, string salt) => throw new System.NotImplementedException();
    public Session Authenticate(string username, string password) => throw new System.NotImplementedException();
}

public class AuthorizationManager : IAuthorizationManager
{
    private IMetadataCatalog catalog;
    
    private HashSet<RoleId> ResolveRoles(UserId user) => throw new System.NotImplementedException();
    
    public void CheckPermission(UserId user, ObjectId obj, Action action) => throw new System.NotImplementedException();
    public void GrantRole(UserId user, RoleId role) => throw new System.NotImplementedException();
    public void RevokeRole(UserId user, RoleId role) => throw new System.NotImplementedException();
}
