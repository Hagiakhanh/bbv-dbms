using OnlineStore.DTOs.Store;
using OnlineStore.Repositories.Store;

namespace OnlineStore.Services.Store
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<StoreResponse?> GetStoreAsync()
        {
            var store = await _storeRepository.GetStoreAsync();
            if (store == null)
            {
                return null;
            }

            return new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
                Plan = store.Plan,
                LiveStatus = store.LiveStatus,
                StorefrontUrl = store.StorefrontUrl
            };
        }

        public async Task<UnreadCountResponse> GetUnreadNotificationCountAsync()
        {
            var count = await _storeRepository.GetUnreadNotificationCountAsync();
            return new UnreadCountResponse
            {
                UnreadCount = count
            };
        }
    }
}
