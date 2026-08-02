using OnlineStore.DTOs.Auth;
using OnlineStore.Entities;

namespace OnlineStore.Services.Users
{
    public interface ICurrentUserService
    {
        Task<CurrentUserResponse?> GetCurrentUserAsync(string userId, bool includeRole = true, bool includeStore = true);
        Task<CurrentUserResponse> BuildCurrentUserDtoAsync(User user, bool includeRole = true, bool includeStore = true);
    }
}
