using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DBMS.Domain.Security;

public class SecurityManager : ISecurityManager
{
    private readonly ConcurrentDictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, List<string>> _userRoles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, HashSet<string>> _rolePermissions = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _dbOwners = new(StringComparer.OrdinalIgnoreCase);

    public SecurityManager()
    {
        // Seed default admin user
        var adminUser = new User
        {
            UserId = 1,
            Username = "admin",
            Email = "admin@dbms.local",
            PasswordHash = HashPassword("password123"),
            IsActive = true,
            Roles = new List<string> { "sysadmin", "db_owner" }
        };
        _users[adminUser.Username] = adminUser;
        _userRoles[adminUser.Username] = new List<string> { "sysadmin", "db_owner" };

        // Seed default roles & permissions
        _rolePermissions["sysadmin"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "*" };
        _rolePermissions["db_owner"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", "ALTER" };

        // Seed default regular user for testing
        var defaultUser = new User
        {
            UserId = 2,
            Username = "user",
            Email = "user@dbms.local",
            PasswordHash = HashPassword("pass"),
            IsActive = true,
            Roles = new List<string> { "user" }
        };
        _users[defaultUser.Username] = defaultUser;
        _userRoles[defaultUser.Username] = new List<string> { "user" };

        var user1 = new User
        {
            UserId = 3,
            Username = "user1",
            Email = "user1@dbms.local",
            PasswordHash = HashPassword("pass123"),
            IsActive = true,
            Roles = new List<string> { "user" }
        };
        _users[user1.Username] = user1;
        _userRoles[user1.Username] = new List<string> { "user" };
        _rolePermissions["user"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SELECT", "INSERT", "UPDATE" };
    }

    public User? Authenticate(string username, string password)
    {
        if (!_users.TryGetValue(username, out var user))
        {
            throw new UnauthorizedAccessException($"User '{username}' does not exist.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException($"User '{username}' is inactive.");
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid password.");
        }

        return user;
    }

    public bool CheckPermission(string user, int obj, string action)
    {
        return HasPermission(user, obj, action);
    }

    public bool HasPermission(string user, int obj, string action)
    {
        if (string.Equals(user, "guest", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!_users.TryGetValue(user, out _))
        {
            return false;
        }

        if (string.Equals(user, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (_userRoles.TryGetValue(user, out var roles))
        {
            foreach (var role in roles)
            {
                if (_rolePermissions.TryGetValue(role, out var perms))
                {
                    if (perms.Contains("*") || perms.Contains(action))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool Authorize(string user, int obj, string action)
    {
        if (!HasPermission(user, obj, action))
        {
            throw new UnauthorizedAccessException($"User '{user}' is not authorized to perform '{action}' on object {obj}.");
        }
        return true;
    }

    public void GrantRole(string user, string role)
    {
        var roles = _userRoles.GetOrAdd(user, _ => new List<string>());
        lock (roles)
        {
            if (!roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                roles.Add(role);
            }
        }

        if (_users.TryGetValue(user, out var u))
        {
            if (!u.Roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                u.Roles.Add(role);
            }
        }
    }

    public void RevokeRole(string user, string role)
    {
        if (_userRoles.TryGetValue(user, out var roles))
        {
            lock (roles)
            {
                roles.RemoveAll(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
            }
        }

        if (_users.TryGetValue(user, out var u))
        {
            u.Roles.RemoveAll(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void GrantOwnership(string dbName, string owner)
    {
        _dbOwners[dbName] = owner;
    }

    public static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return string.Equals(hash, storedHash, StringComparison.Ordinal);
    }
}
