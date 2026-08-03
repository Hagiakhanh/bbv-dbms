using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBMS.Domain.Security;

namespace DBMS.API.Repositories.Roles;

public class InMemoryRoleRepository : IRoleRepository
{
    private readonly ConcurrentDictionary<int, Role> _roles = new();
    private int _nextId = 1;

    public InMemoryRoleRepository()
    {
        // Seed default roles
        var sysAdmin = new Role
        {
            RoleId = _nextId++,
            Name = "sysadmin",
            Description = "System Administrator with full permissions",
            Permissions = new List<string> { "*", "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER" }
        };
        _roles[sysAdmin.RoleId] = sysAdmin;

        var dbOwner = new Role
        {
            RoleId = _nextId++,
            Name = "db_owner",
            Description = "Database Owner with administrative permissions over databases",
            Permissions = new List<string> { "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER" }
        };
        _roles[dbOwner.RoleId] = dbOwner;

        var userRole = new Role
        {
            RoleId = _nextId++,
            Name = "user",
            Description = "Standard database user",
            Permissions = new List<string> { "SELECT", "INSERT", "UPDATE" }
        };
        _roles[userRole.RoleId] = userRole;
    }

    public Task<Role?> GetByIdAsync(int roleId)
    {
        _roles.TryGetValue(roleId, out var role);
        return Task.FromResult(role);
    }

    public Task<Role?> GetByNameAsync(string name)
    {
        var role = _roles.Values.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(role);
    }

    public Task<IEnumerable<Role>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Role>>(_roles.Values.ToList());
    }

    public Task<Role> CreateAsync(Role role)
    {
        if (role.RoleId <= 0)
        {
            role.RoleId = System.Threading.Interlocked.Increment(ref _nextId);
        }
        _roles[role.RoleId] = role;
        return Task.FromResult(role);
    }

    public Task<Role> UpdateAsync(Role role)
    {
        _roles[role.RoleId] = role;
        return Task.FromResult(role);
    }

    public Task<bool> DeleteAsync(int roleId)
    {
        return Task.FromResult(_roles.TryRemove(roleId, out _));
    }
}
