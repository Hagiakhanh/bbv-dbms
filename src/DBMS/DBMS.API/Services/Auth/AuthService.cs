using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBMS.API.DTOs.Auth;
using DBMS.API.Repositories.Roles;
using DBMS.API.Repositories.Users;
using DBMS.Domain.Security;

namespace DBMS.API.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenService _tokenService;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IJwtTokenService tokenService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
    }

    public async Task<UserInfoDto> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password cannot be empty.");
        }

        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with username '{request.Username}' already exists.");
        }

        var roles = request.Roles != null && request.Roles.Count > 0 ? request.Roles : new List<string> { "user" };

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email ?? string.Empty,
            PasswordHash = SecurityManager.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Roles = roles
        };

        var created = await _userRepository.CreateAsync(newUser);

        return new UserInfoDto
        {
            UserId = created.UserId,
            Username = created.Username,
            Email = created.Email,
            IsActive = created.IsActive,
            Roles = created.Roles
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        if (!SecurityManager.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.Roles
            }
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new ArgumentException("Refresh token is required.");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var tokenItem = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);
        if (tokenItem == null || tokenItem.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // Revoke old token and generate new ones
        tokenItem.IsRevoked = true;

        var (newAccessToken, expiresAt) = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        });

        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.Roles
            }
        };
    }

    public async Task<bool> LogoutAsync(string username, string? refreshToken)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
            }
        }
        else
        {
            foreach (var rt in user.RefreshTokens)
            {
                rt.IsRevoked = true;
            }
        }

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<UserInfoDto?> GetCurrentUserAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return null;

        return new UserInfoDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = user.Roles
        };
    }
}
