using OnlineStore.DTOs.Store;

namespace OnlineStore.Services.Store
{
    public interface IStoreService
    {
        Task<StoreResponse?> GetStoreAsync();
        Task<UnreadCountResponse> GetUnreadNotificationCountAsync();
    }
}
