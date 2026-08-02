using OnlineStore.DTOs;

namespace OnlineStore.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);
        Task<MessageResponse> LogoutAsync(string userId, bool allDevices);
        Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, bool includeRole = true, bool includeStore = true);
    }
}
