using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBMS.Domain.Security;

namespace DBMS.API.Repositories.Users;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId = 1;

    public InMemoryUserRepository()
    {
        // Seed default admin user
        var admin = new User
        {
            UserId = _nextId++,
            Username = "admin",
            Email = "admin@dbms.local",
            PasswordHash = SecurityManager.HashPassword("Admin@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Roles = new List<string> { "sysadmin", "db_owner" }
        };
        _users[admin.UserId] = admin;
    }

    public Task<User?> GetByIdAsync(int userId)
    {
        _users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _users.Values.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        var user = _users.Values.FirstOrDefault(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow));
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<User>>(_users.Values.ToList());
    }

    public Task<User> CreateAsync(User user)
    {
        if (user.UserId <= 0)
        {
            user.UserId = System.Threading.Interlocked.Increment(ref _nextId);
        }
        _users[user.UserId] = user;
        return Task.FromResult(user);
    }

    public Task<User> UpdateAsync(User user)
    {
        _users[user.UserId] = user;
        return Task.FromResult(user);
    }

    public Task<bool> DeleteAsync(int userId)
    {
        return Task.FromResult(_users.TryRemove(userId, out _));
    }
}
