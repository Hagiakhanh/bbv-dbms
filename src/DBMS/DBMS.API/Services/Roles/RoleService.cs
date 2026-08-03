using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBMS.API.DTOs.Roles;
using DBMS.API.Repositories.Roles;
using DBMS.API.Repositories.Users;
using DBMS.Domain.Security;

namespace DBMS.API.Services.Roles;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleService(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Role name cannot be empty.");
        }

        var existingRole = await _roleRepository.GetByNameAsync(request.Name);
        if (existingRole != null)
        {
            throw new InvalidOperationException($"Role '{request.Name}' already exists.");
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Permissions = request.Permissions ?? new List<string>()
        };

        var created = await _roleRepository.CreateAsync(role);
        return MapToDto(created);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        return role == null ? null : MapToDto(role);
    }

    public async Task<bool> AssignRolesToUserAsync(int userId, List<string> roles)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        foreach (var roleName in roles)
        {
            if (!user.Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
            {
                user.Roles.Add(roleName);
            }
        }

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found.");
        }

        user.Roles.RemoveAll(r => string.Equals(r, role.Name, StringComparison.OrdinalIgnoreCase));
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<RoleDto> GrantPermissionsToRoleAsync(int roleId, List<string> permissions)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found.");
        }

        foreach (var perm in permissions)
        {
            if (!role.Permissions.Contains(perm, StringComparer.OrdinalIgnoreCase))
            {
                role.Permissions.Add(perm);
            }
        }

        var updated = await _roleRepository.UpdateAsync(role);
        return MapToDto(updated);
    }

    public async Task<RoleDto> RevokePermissionFromRoleAsync(int roleId, string permission)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {roleId} not found.");
        }

        role.Permissions.RemoveAll(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase));
        var updated = await _roleRepository.UpdateAsync(role);
        return MapToDto(updated);
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            RoleId = role.RoleId,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.Permissions
        };
    }
}
