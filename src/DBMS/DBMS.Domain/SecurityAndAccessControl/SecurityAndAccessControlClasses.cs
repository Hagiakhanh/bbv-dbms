namespace DBMS.Domain.SecurityAndAccessControl;

using System.Collections.Generic;
using DBMS.Domain.DatabaseObjectManagement;

public class AuthenticationManager
{
    public Dictionary<string, object> UserDb { get; set; }

    public void Authenticate() => throw new System.NotImplementedException();
    public void HashPassword() => throw new System.NotImplementedException();
}

public class AuthorizationManager
{
    public MetadataCatalog Catalog { get; set; }

    public void CheckPermission() => throw new System.NotImplementedException();
    public void GrantRole() => throw new System.NotImplementedException();
}
