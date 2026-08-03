using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Users;
using DBMS.API.Repositories.Users;
using DBMS.API.Services.Users;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly IUserRepository _userRepository;

    public UserServiceTests()
    {
        _userRepository = new InMemoryUserRepository();
        _userService = new UserService(_userRepository);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUserList()
    {
        var users = await _userService.GetAllUsersAsync();

        users.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingId_ReturnsUserDto()
    {
        var user = await _userService.GetUserByIdAsync(1);

        user.Should().NotBeNull();
        user!.Username.Should().Be("admin");
    }

    [Fact]
    public async Task CreateUserAsync_CreatesNewUser()
    {
        var request = new CreateUserRequest
        {
            Username = "srv_user_create",
            Password = "Pass123!Password",
            Email = "srv@test.com",
            Roles = new List<string> { "user" }
        };

        var created = await _userService.CreateUserAsync(request);

        created.Should().NotBeNull();
        created.UserId.Should().BeGreaterThan(0);
        created.Username.Should().Be("srv_user_create");
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUserEmailAndStatus()
    {
        var updateRequest = new UpdateUserRequest
        {
            Email = "updated_user_email@test.com",
            IsActive = false
        };

        var updated = await _userService.UpdateUserAsync(1, updateRequest);

        updated.Email.Should().Be("updated_user_email@test.com");
        updated.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_DeletesUser()
    {
        var created = await _userService.CreateUserAsync(new CreateUserRequest { Username = "to_delete", Password = "password" });

        var result = await _userService.DeleteUserAsync(created.UserId);

        result.Should().BeTrue();
        var fetched = await _userService.GetUserByIdAsync(created.UserId);
        fetched.Should().BeNull();
    }
}
