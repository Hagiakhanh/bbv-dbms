using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Roles;
using DBMS.API.Repositories.Roles;
using DBMS.API.Repositories.Users;
using DBMS.API.Services.Roles;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Services;

public class RoleServiceTests
{
    private readonly RoleService _roleService;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleServiceTests()
    {
        _roleRepository = new InMemoryRoleRepository();
        _userRepository = new InMemoryUserRepository();
        _roleService = new RoleService(_roleRepository, _userRepository);
    }

    [Fact]
    public async Task GetAllRolesAsync_ReturnsSeededRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();

        roles.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateRoleAsync_WithValidRequest_CreatesRole()
    {
        var request = new CreateRoleRequest
        {
            Name = "security_officer",
            Description = "Security Officer",
            Permissions = new List<string> { "AUDIT_LOGS" }
        };

        var created = await _roleService.CreateRoleAsync(request);

        created.Should().NotBeNull();
        created.Name.Should().Be("security_officer");
        created.Permissions.Should().Contain("AUDIT_LOGS");
    }

    [Fact]
    public async Task AssignRolesToUserAsync_AssignsRoleToUser()
    {
        var result = await _roleService.AssignRolesToUserAsync(1, new List<string> { "user" });

        result.Should().BeTrue();
        var user = await _userRepository.GetByIdAsync(1);
        user!.Roles.Should().Contain("user");
    }

    [Fact]
    public async Task GrantPermissionsToRoleAsync_GrantsPermissionToRole()
    {
        var role = await _roleRepository.GetByNameAsync("user");
        role.Should().NotBeNull();

        var updated = await _roleService.GrantPermissionsToRoleAsync(role!.RoleId, new List<string> { "DELETE_OWN_RECORDS" });

        updated.Permissions.Should().Contain("DELETE_OWN_RECORDS");
    }

    [Fact]
    public async Task RevokePermissionFromRoleAsync_RevokesPermission()
    {
        var role = await _roleRepository.GetByNameAsync("user");
        role.Should().NotBeNull();

        var updated = await _roleService.RevokePermissionFromRoleAsync(role!.RoleId, "SELECT");

        updated.Permissions.Should().NotContain("SELECT");
    }
}
