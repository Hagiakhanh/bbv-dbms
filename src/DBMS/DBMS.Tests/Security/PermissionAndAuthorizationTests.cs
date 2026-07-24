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

        Action act = () => secMgr.HasPermission("admin", 1, "SELECT");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void HasPermission_ShouldReturnFalse_WhenPermissionDoesNotExist()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.HasPermission("guest", 1, "DELETE");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Authorize_ShouldAllowAuthorizedUser()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.Authorize("admin", 1, "SELECT");

        act.Should().Throw<NotImplementedException>();
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

        Action act = () => secMgr.Authorize("user1", 10, "UPDATE");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Authorize_ShouldVerifyUserPermission()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.Authorize("user1", 10, "INSERT");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void GrantRole_ShouldAssignRoleToUser()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.GrantRole("user1", "db_owner");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RevokeRole_ShouldRemoveRoleFromUser()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.RevokeRole("user1", "db_owner");

        act.Should().Throw<NotImplementedException>();
    }
}
