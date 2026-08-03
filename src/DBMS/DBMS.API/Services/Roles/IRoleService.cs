using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Roles;

namespace DBMS.API.Services.Roles;

public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(CreateRoleRequest request);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int roleId);
    Task<bool> AssignRolesToUserAsync(int userId, List<string> roles);
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    Task<RoleDto> GrantPermissionsToRoleAsync(int roleId, List<string> permissions);
    Task<RoleDto> RevokePermissionFromRoleAsync(int roleId, string permission);
}
