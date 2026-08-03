using System;
using System.Collections.Generic;
using DBMS.API.Services.Auth;
using DBMS.Domain.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DBMS.Tests.API.Services;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Secret", "DBMS_SuperSecretKey_For_JwtTokens_2026_Authentication!"},
            {"Jwt:Issuer", "DBMS.API"},
            {"Jwt:Audience", "DBMS.Clients"},
            {"Jwt:ExpiryMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _jwtTokenService = new JwtTokenService(configuration);
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidJwtStringAndExpiration()
    {
        var user = new User
        {
            UserId = 10,
            Username = "jwtuser",
            Email = "jwt@test.com",
            Roles = new List<string> { "sysadmin", "user" }
        };

        var (token, expiresAt) = _jwtTokenService.GenerateAccessToken(user);

        token.Should().NotBeNullOrWhiteSpace();
        token.Split('.').Length.Should().Be(3); // Standard JWT header.payload.signature format
        expiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsRandomString()
    {
        var refreshToken1 = _jwtTokenService.GenerateRefreshToken();
        var refreshToken2 = _jwtTokenService.GenerateRefreshToken();

        refreshToken1.Should().NotBeNullOrWhiteSpace();
        refreshToken2.Should().NotBeNullOrWhiteSpace();
        refreshToken1.Should().NotBe(refreshToken2);
    }
}
