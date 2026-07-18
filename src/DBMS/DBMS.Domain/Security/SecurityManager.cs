using System;
using System.Collections.Generic;

namespace DBMS.Domain.Security;

public class SecurityManager
{
    private Dictionary<string, object> userDb;

    public object Authenticate(string username, string password)
    {
        throw new NotImplementedException();
    }

    public bool CheckPermission(string user, int obj, string action)
    {
        throw new NotImplementedException();
    }

    public void GrantRole(string user, string role)
    {
        throw new NotImplementedException();
    }

    public void RevokeRole(string user, string role)
    {
        throw new NotImplementedException();
    }
}
