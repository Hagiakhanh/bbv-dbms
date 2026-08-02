using System.Security.Claims;
using OnlineStore.DTOs.Auth;
using OnlineStore.DTOs.Common;
using OnlineStore.Repositories.Users;
using OnlineStore.Services.Tokens;
using OnlineStore.Services.Users;

namespace OnlineStore.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername);
            if (user == null || user.PasswordHash != request.Password)
            {
                return null;
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var currentUserDto = await _currentUserService.BuildCurrentUserDtoAsync(user, true, true);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                User = currentUserDto
            };
        }

        public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? principal.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(userIdClaim);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var currentUserDto = await _currentUserService.BuildCurrentUserDtoAsync(user, true, true);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                User = currentUserDto
            };
        }

        public async Task<MessageResponse> LogoutAsync(string userId, bool allDevices)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userRepository.UpdateAsync(user);
            }

            return new MessageResponse
            {
                Success = true,
                Message = allDevices ? "Logged out successfully from all devices." : "Logged out successfully."
            };
        }
    }
}
