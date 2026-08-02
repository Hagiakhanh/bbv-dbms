using OnlineStore.DTOs.Auth;
using OnlineStore.DTOs.Store;
using OnlineStore.Entities;
using OnlineStore.Repositories.Store;
using OnlineStore.Repositories.Users;

namespace OnlineStore.Services.Users
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;

        public CurrentUserService(IUserRepository userRepository, IStoreRepository storeRepository)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
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

        public async Task<CurrentUserResponse> BuildCurrentUserDtoAsync(User user, bool includeRole = true, bool includeStore = true)
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
    }
}
