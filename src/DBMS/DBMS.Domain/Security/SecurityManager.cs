using System;
using System.Collections.Generic;

namespace DBMS.Domain.Security;

public class SecurityManager
{
    private Map<string, HashedCredential<> userDb;
    public Session Authenticate(username : string, password : string) { throw new NotImplementedException(); }
    public bool CheckPermission(user : string, obj : int, action : string) { throw new NotImplementedException(); }
    public void GrantRole(user : string, role : string) { throw new NotImplementedException(); }
    public void RevokeRole(user : string, role : string) { throw new NotImplementedException(); }
}
