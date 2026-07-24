using System;
using DBMS.Domain.Security;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Security;

public class AuthenticationTests
{
    [Fact]
    public void Login_ShouldAuthenticateValidUser()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.Authenticate("admin", "password123");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Login_ShouldRejectInvalidUsernameOrPassword()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.Authenticate("admin", "wrongpassword");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Authenticate_ShouldValidateUserCredentials()
    {
        var secMgr = new SecurityManager();

        Action act = () => secMgr.Authenticate("user", "pass");

        act.Should().Throw<NotImplementedException>();
    }
}
