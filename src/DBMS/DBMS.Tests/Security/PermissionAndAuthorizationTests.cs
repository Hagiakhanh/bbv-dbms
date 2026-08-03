using System;
using DBMS.Domain.Security;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Security;

public class PermissionAndAuthorizationTests
{
    [Fact]
    public void HasPermission_ShouldReturnTrue_WhenPermissionExists()
    {
        var secMgr = new SecurityManager();

        var result = secMgr.HasPermission("admin", 1, "SELECT");

        result.Should().BeTrue();
    }

    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenPermissionDoesNotExist()
    {
        var secMgr = new SecurityManager();

        var result = secMgr.HasPermission("guest", 1, "DELETE");

        result.Should().BeFalse();
    }

    [Fact]
    public void Authorize_ShouldAllowAuthorizedUser()
    {
        var secMgr = new SecurityManager();

        var result = secMgr.Authorize("admin", 1, "SELECT");

        result.Should().BeTrue();
    }

    [Fact]
    public void Authorize_ShouldRejectUnauthorizedUser()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.Authorize("guest", 1, "DROP");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Authorize_ShouldCheckObjectPermissions()
    {
        var secMgr = new SecurityManager();

        var result = secMgr.Authorize("user1", 10, "UPDATE");

        result.Should().BeTrue();
    }

    [Fact]
    public void Authorize_ShouldVerifyUserPermission()
    {
        var secMgr = new SecurityManager();

        var result = secMgr.Authorize("user1", 10, "INSERT");

        result.Should().BeTrue();
    }

    [Fact]
    public void GrantRole_ShouldAssignRoleToUser()
    {
        var secMgr = new SecurityManager();

        secMgr.GrantRole("user1", "db_owner");

        secMgr.HasPermission("user1", 1, "DROP").Should().BeTrue();
    }

    [Fact]
    public void RevokeRole_ShouldRemoveRoleFromUser()
    {
        var secMgr = new SecurityManager();
        secMgr.GrantRole("user1", "db_owner");
        secMgr.RevokeRole("user1", "db_owner");

        secMgr.HasPermission("user1", 1, "DROP").Should().BeFalse();
    }
}
