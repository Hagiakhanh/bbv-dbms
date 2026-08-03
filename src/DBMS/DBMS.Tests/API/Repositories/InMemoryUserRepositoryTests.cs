using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBMS.API.Repositories.Users;
using DBMS.Domain.Security;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories;

public class InMemoryUserRepositoryTests
{
    private readonly InMemoryUserRepository _repository;

    public InMemoryUserRepositoryTests()
    {
        _repository = new InMemoryUserRepository();
    }

    [Fact]
    public async Task GetByIdAsync_WithSeededAdminId_ReturnsAdminUser()
    {
        var admin = await _repository.GetByIdAsync(1);

        admin.Should().NotBeNull();
        admin!.Username.Should().Be("admin");
    }

    [Fact]
    public async Task GetByUsernameAsync_WithExistingUsername_ReturnsUser()
    {
        var user = await _repository.GetByUsernameAsync("admin");

        user.Should().NotBeNull();
        user!.Username.Should().Be("admin");
    }

    [Fact]
    public async Task GetByUsernameAsync_WithNonExistingUsername_ReturnsNull()
    {
        var user = await _repository.GetByUsernameAsync("nonexistent");

        user.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsNewUserSuccessfully()
    {
        var newUser = new User
        {
            Username = "repo_test_user",
            Email = "repo@test.com",
            PasswordHash = "hash123",
            IsActive = true
        };

        var created = await _repository.CreateAsync(newUser);

        created.UserId.Should().BeGreaterThan(0);
        
        var fetched = await _repository.GetByIdAsync(created.UserId);
        fetched.Should().NotBeNull();
        fetched!.Username.Should().Be("repo_test_user");
    }

    [Fact]
    public async Task UpdateAsync_ModifiesUserProperties()
    {
        var user = await _repository.GetByUsernameAsync("admin");
        user.Should().NotBeNull();

        user!.Email = "updated_admin@test.com";
        await _repository.UpdateAsync(user);

        var updated = await _repository.GetByUsernameAsync("admin");
        updated!.Email.Should().Be("updated_admin@test.com");
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserFromRepository()
    {
        var newUser = new User
        {
            Username = "delete_test_user",
            Email = "delete@test.com",
            PasswordHash = "hash123"
        };
        var created = await _repository.CreateAsync(newUser);

        var deleted = await _repository.DeleteAsync(created.UserId);

        deleted.Should().BeTrue();
        var fetched = await _repository.GetByIdAsync(created.UserId);
        fetched.Should().BeNull();
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_ReturnsUserWithMatchingActiveRefreshToken()
    {
        var newUser = new User
        {
            Username = "refreshtoken_user",
            Email = "rt@test.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Token = "valid_token_123",
                    ExpiresAt = DateTime.UtcNow.AddDays(1),
                    IsRevoked = false
                }
            }
        };
        await _repository.CreateAsync(newUser);

        var user = await _repository.GetByRefreshTokenAsync("valid_token_123");

        user.Should().NotBeNull();
        user!.Username.Should().Be("refreshtoken_user");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var users = await _repository.GetAllAsync();

        users.Should().NotBeEmpty();
        users.Any(u => u.Username == "admin").Should().BeTrue();
    }
}
