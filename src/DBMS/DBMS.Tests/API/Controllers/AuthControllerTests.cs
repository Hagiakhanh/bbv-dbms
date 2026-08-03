using System.Security.Claims;
using System.Threading.Tasks;
using DBMS.API.Controllers;
using DBMS.API.DTOs.Auth;
using DBMS.API.Repositories.Roles;
using DBMS.API.Repositories.Users;
using DBMS.API.Services.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DBMS.Tests.API.Controllers;

public class AuthControllerTests
{
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        var userRepo = new InMemoryUserRepository();
        var roleRepo = new InMemoryRoleRepository();
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var tokenService = new JwtTokenService(config);
        var authService = new AuthService(userRepo, roleRepo, tokenService);
        _authController = new AuthController(authService);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "Admin@123"
        };

        var result = await _authController.Login(request);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponse>().Subject;

        response.AccessToken.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
        response.User.Username.Should().Be("admin");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var request = new LoginRequest
        {
            Username = "admin",
            Password = "WrongPassword"
        };

        var result = await _authController.Login(request);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Register_WithNewUser_ReturnsCreated()
    {
        var request = new RegisterRequest
        {
            Username = "newtestuser",
            Password = "Password123!",
            Email = "newuser@test.com"
        };

        var result = await _authController.Register(request);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var user = createdResult.Value.Should().BeOfType<UserInfoDto>().Subject;

        user.Username.Should().Be("newtestuser");
        user.Email.Should().Be("newuser@test.com");
    }

    [Fact]
    public async Task Register_WithExistingUsername_ReturnsConflict()
    {
        var request = new RegisterRequest
        {
            Username = "admin",
            Password = "Password123!"
        };

        var result = await _authController.Register(request);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ReturnsOk()
    {
        var loginResult = await _authController.Login(new LoginRequest { Username = "admin", Password = "Admin@123" });
        var okLogin = (OkObjectResult)loginResult.Result!;
        var loginResponse = (AuthResponse)okLogin.Value!;

        var refreshResult = await _authController.RefreshToken(new RefreshTokenRequest { RefreshToken = loginResponse.RefreshToken });

        var okRefresh = refreshResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var refreshResponse = okRefresh.Value.Should().BeOfType<AuthResponse>().Subject;
        refreshResponse.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidRefreshToken_ReturnsUnauthorized()
    {
        var refreshResult = await _authController.RefreshToken(new RefreshTokenRequest { RefreshToken = "invalid_token" });

        refreshResult.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetMe_WithAuthenticatedClaims_ReturnsUserInfo()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "admin") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _authController.GetMe();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var user = okResult.Value.Should().BeOfType<UserInfoDto>().Subject;
        user.Username.Should().Be("admin");
    }

    [Fact]
    public async Task Logout_WithAuthenticatedClaims_ReturnsNoContent()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "admin") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await _authController.Logout(new LogoutRequest());

        result.Should().BeOfType<NoContentResult>();
    }
}
