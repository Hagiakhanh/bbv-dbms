using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBMS.API.Repositories.Roles;
using DBMS.Domain.Security;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.API.Repositories;

public class InMemoryRoleRepositoryTests
{
    private readonly InMemoryRoleRepository _repository;

    public InMemoryRoleRepositoryTests()
    {
        _repository = new InMemoryRoleRepository();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSeededRoles()
    {
        var roles = await _repository.GetAllAsync();

        roles.Should().NotBeEmpty();
        roles.Select(r => r.Name).Should().Contain(new[] { "sysadmin", "db_owner", "user" });
    }

    [Fact]
    public async Task GetByNameAsync_WithExistingName_ReturnsRole()
    {
        var role = await _repository.GetByNameAsync("sysadmin");

        role.Should().NotBeNull();
        role!.Name.Should().Be("sysadmin");
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistingName_ReturnsNull()
    {
        var role = await _repository.GetByNameAsync("nonexistent_role");

        role.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsNewRoleSuccessfully()
    {
        var newRole = new Role
        {
            Name = "auditor",
            Description = "Audit manager",
            Permissions = new List<string> { "READ_AUDIT" }
        };

        var created = await _repository.CreateAsync(newRole);

        created.RoleId.Should().BeGreaterThan(0);
        var fetched = await _repository.GetByIdAsync(created.RoleId);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("auditor");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesRolePermissions()
    {
        var role = await _repository.GetByNameAsync("user");
        role.Should().NotBeNull();

        role!.Permissions.Add("EXECUTE");
        await _repository.UpdateAsync(role);

        var updated = await _repository.GetByNameAsync("user");
        updated!.Permissions.Should().Contain("EXECUTE");
    }

    [Fact]
    public async Task DeleteAsync_RemovesRole()
    {
        var role = new Role
        {
            Name = "temp_role",
            Description = "Temporary role"
        };
        var created = await _repository.CreateAsync(role);

        var deleted = await _repository.DeleteAsync(created.RoleId);

        deleted.Should().BeTrue();
        var fetched = await _repository.GetByIdAsync(created.RoleId);
        fetched.Should().BeNull();
    }
}
