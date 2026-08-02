using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.DTOs;
using OnlineStore.Entities;
using OnlineStore.Repositories;

namespace OnlineStore.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IStoreRepository storeRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername);
            if (user == null || user.PasswordHash != request.Password)
            {
                return null;
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var currentUserDto = await BuildCurrentUserDtoAsync(user, true, true);

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
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(userIdClaim);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var currentUserDto = await BuildCurrentUserDtoAsync(user, true, true);

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

        public async Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, bool includeRole = true, bool includeStore = true)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return await BuildCurrentUserDtoAsync(user, includeRole, includeStore);
        }

        private async Task<CurrentUserResponse> BuildCurrentUserDtoAsync(User user, bool includeRole, bool includeStore)
        {
            StoreResponse? storeDto = null;
            if (includeStore)
            {
                var store = await _storeRepository.GetStoreAsync();
                if (store != null)
                {
                    storeDto = new StoreResponse
                    {
                        Id = store.Id,
                        Name = store.Name,
                        Plan = store.Plan,
                        LiveStatus = store.LiveStatus,
                        StorefrontUrl = store.StorefrontUrl
                    };
                }
            }

            return new CurrentUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = includeRole ? user.Role : string.Empty,
                AvatarUrl = user.AvatarUrl,
                StoreId = user.StoreId,
                Store = storeDto
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKeyForOnlineStoreManagementApi2026!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "OnlineStoreAPI",
                audience: _configuration["Jwt:Audience"] ?? "OnlineStoreClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKeyForOnlineStoreManagementApi2026!";
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateLifetime = false // Here we want to validate an expired token for refresh
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
