using System.Threading.Tasks;
using DBMS.API.DTOs.Auth;

namespace DBMS.API.Services.Auth;

public interface IAuthService
{
    Task<UserInfoDto> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> LogoutAsync(string username, string? refreshToken);
    Task<UserInfoDto?> GetCurrentUserAsync(string username);
}
