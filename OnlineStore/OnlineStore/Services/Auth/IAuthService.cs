using OnlineStore.DTOs.Auth;
using OnlineStore.DTOs.Common;

namespace OnlineStore.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task<MessageResponse> LogoutAsync(string userId, bool allDevices);
    }
}
