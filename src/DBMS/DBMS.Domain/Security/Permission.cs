using System;

namespace DBMS.Domain.Security;

public class Permission
{
    public int PermissionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public int ObjectId { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
