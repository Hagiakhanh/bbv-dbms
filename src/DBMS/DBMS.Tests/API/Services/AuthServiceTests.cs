using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Auth;
using DBMS.API.Repositories.Roles;
using DBMS.API.Repositories.Users;
using DBMS.API.Services.Auth;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DBMS.Tests.API.Services;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public AuthServiceTests()
    {
        _userRepository = new InMemoryUserRepository();
        _roleRepository = new InMemoryRoleRepository();
        
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var tokenService = new JwtTokenService(config);

        _authService = new AuthService(_userRepository, _roleRepository, tokenService);
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_CreatesUser()
    {
        var request = new RegisterRequest
        {
            Username = "auth_reg_user",
            Password = "Password123!",
            Email = "auth_reg@test.com",
            Roles = new List<string> { "user" }
        };

        var user = await _authService.RegisterAsync(request);

        user.Should().NotBeNull();
        user.Username.Should().Be("auth_reg_user");
        user.Email.Should().Be("auth_reg@test.com");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ThrowsInvalidOperationException()
    {
        var request = new RegisterRequest
        {
            Username = "admin",
            Password = "Password123!"
        };

        Func<Task> act = async () => await _authService.RegisterAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthTokens()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "Admin@123"
        };

        var response = await _authService.LoginAsync(request);

        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
        response.User.Username.Should().Be("admin");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "WrongPassword"
        };

        Func<Task> act = async () => await _authService.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ReturnsNewTokens()
    {
        var loginResponse = await _authService.LoginAsync(new LoginRequest { Username = "admin", Password = "Admin@123" });

        var refreshResponse = await _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = loginResponse.RefreshToken });

        refreshResponse.Should().NotBeNull();
        refreshResponse.AccessToken.Should().NotBeNullOrEmpty();
        refreshResponse.RefreshToken.Should().NotBeNullOrEmpty();
        refreshResponse.RefreshToken.Should().NotBe(loginResponse.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ThrowsUnauthorizedAccessException()
    {
        Func<Task> act = async () => await _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = "invalid_token" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LogoutAsync_RevokesRefreshTokens()
    {
        var loginResponse = await _authService.LoginAsync(new LoginRequest { Username = "admin", Password = "Admin@123" });

        var result = await _authService.LogoutAsync("admin", loginResponse.RefreshToken);

        result.Should().BeTrue();

        Func<Task> act = async () => await _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = loginResponse.RefreshToken });
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUserInformation()
    {
        var user = await _authService.GetCurrentUserAsync("admin");

        user.Should().NotBeNull();
        user!.Username.Should().Be("admin");
        user.Roles.Should().Contain("sysadmin");
    }
}
