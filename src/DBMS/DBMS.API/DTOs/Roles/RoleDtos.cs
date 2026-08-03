using System;
using System.Collections.Generic;

namespace DBMS.API.DTOs.Roles;

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? Permissions { get; set; }
}

public class RoleDto
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class AssignRoleRequest
{
    public List<string> Roles { get; set; } = new();
}

public class GrantPermissionRequest
{
    public List<string> Permissions { get; set; } = new();
}
