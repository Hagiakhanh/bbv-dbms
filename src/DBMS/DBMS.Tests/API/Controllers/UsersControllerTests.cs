using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.Controllers;
using DBMS.API.DTOs.Users;
using DBMS.API.Repositories.Users;
using DBMS.API.Services.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DBMS.Tests.API.Controllers;

public class UsersControllerTests
{
    private readonly UsersController _usersController;

    public UsersControllerTests()
    {
        var userRepo = new InMemoryUserRepository();
        var userService = new UserService(userRepo);
        _usersController = new UsersController(userService);
    }

    [Fact]
    public async Task GetUsers_ReturnsListOfUsers()
    {
        var result = await _usersController.GetUsers();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<IEnumerable<UserDto>>().Subject;

        users.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUserById_WithExistingId_ReturnsOkUser()
    {
        var result = await _usersController.GetUserById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var user = okResult.Value.Should().BeOfType<UserDto>().Subject;
        user.Username.Should().Be("admin");
    }

    [Fact]
    public async Task GetUserById_WithNonExistingId_ReturnsNotFound()
    {
        var result = await _usersController.GetUserById(9999);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateUser_WithValidRequest_ReturnsCreated()
    {
        var request = new CreateUserRequest
        {
            Username = "user_create_test",
            Password = "Password123!",
            Email = "test@domain.com"
        };

        var result = await _usersController.CreateUser(request);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var user = createdResult.Value.Should().BeOfType<UserDto>().Subject;

        user.Username.Should().Be("user_create_test");
    }

    [Fact]
    public async Task CreateUser_WithDuplicateUsername_ReturnsConflict()
    {
        var request = new CreateUserRequest
        {
            Username = "admin",
            Password = "Password123!"
        };

        var result = await _usersController.CreateUser(request);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_WithValidRequest_ReturnsUpdatedUser()
    {
        var request = new UpdateUserRequest
        {
            Email = "new_admin_email@test.com"
        };

        var result = await _usersController.UpdateUser(1, request);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var user = okResult.Value.Should().BeOfType<UserDto>().Subject;
        user.Email.Should().Be("new_admin_email@test.com");
    }

    [Fact]
    public async Task UpdateUser_WithNonExistingId_ReturnsNotFound()
    {
        var result = await _usersController.UpdateUser(9999, new UpdateUserRequest());

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteUser_WithExistingId_ReturnsNoContent()
    {
        var createResult = await _usersController.CreateUser(new CreateUserRequest { Username = "delete_target", Password = "Pass" });
        var createdUser = (UserDto)((CreatedAtActionResult)createResult.Result!).Value!;

        var result = await _usersController.DeleteUser(createdUser.UserId);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUser_WithNonExistingId_ReturnsNotFound()
    {
        var result = await _usersController.DeleteUser(9999);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
