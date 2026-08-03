using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.Controllers;
using DBMS.API.DTOs.Roles;
using DBMS.API.Repositories.Roles;
using DBMS.API.Repositories.Users;
using DBMS.API.Services.Roles;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DBMS.Tests.API.Controllers;

public class RolesControllerTests
{
    private readonly RolesController _rolesController;

    public RolesControllerTests()
    {
        var roleRepo = new InMemoryRoleRepository();
        var userRepo = new InMemoryUserRepository();
        var roleService = new RoleService(roleRepo, userRepo);
        _rolesController = new RolesController(roleService);
    }

    [Fact]
    public async Task GetRoles_ReturnsSeededRoles()
    {
        var result = await _rolesController.GetRoles();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var roles = okResult.Value.Should().BeAssignableTo<IEnumerable<RoleDto>>().Subject;

        roles.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetRoleById_WithValidId_ReturnsRole()
    {
        var result = await _rolesController.GetRoleById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var role = okResult.Value.Should().BeOfType<RoleDto>().Subject;
        role.Name.Should().Be("sysadmin");
    }

    [Fact]
    public async Task GetRoleById_WithInvalidId_ReturnsNotFound()
    {
        var result = await _rolesController.GetRoleById(9999);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateRole_ReturnsCreatedRole()
    {
        var request = new CreateRoleRequest
        {
            Name = "analyst",
            Description = "Data Analyst",
            Permissions = new List<string> { "SELECT" }
        };

        var result = await _rolesController.CreateRole(request);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var role = createdResult.Value.Should().BeOfType<RoleDto>().Subject;

        role.Name.Should().Be("analyst");
    }

    [Fact]
    public async Task CreateRole_WithDuplicateName_ReturnsConflict()
    {
        var request = new CreateRoleRequest
        {
            Name = "sysadmin"
        };

        var result = await _rolesController.CreateRole(request);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task AssignRolesToUser_ReturnsOk()
    {
        var request = new AssignRoleRequest
        {
            Roles = new List<string> { "user" }
        };

        var result = await _rolesController.AssignRolesToUser(1, request);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RemoveRoleFromUser_ReturnsNoContent()
    {
        var result = await _rolesController.RemoveRoleFromUser(1, 1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GrantPermissionsToRole_ReturnsOkRole()
    {
        var request = new GrantPermissionRequest
        {
            Permissions = new List<string> { "EXECUTE" }
        };

        var result = await _rolesController.GrantPermissionsToRole(1, request);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var role = okResult.Value.Should().BeOfType<RoleDto>().Subject;
        role.Permissions.Should().Contain("EXECUTE");
    }

    [Fact]
    public async Task RevokePermissionFromRole_ReturnsOkRole()
    {
        var result = await _rolesController.RevokePermissionFromRole(1, "SELECT");

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var role = okResult.Value.Should().BeOfType<RoleDto>().Subject;
        role.Permissions.Should().NotContain("SELECT");
    }
}
