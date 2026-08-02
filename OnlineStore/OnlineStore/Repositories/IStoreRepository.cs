using OnlineStore.Entities;

namespace OnlineStore.Repositories
{
    public interface IStoreRepository
    {
        Task<Store?> GetStoreAsync();
        Task<int> GetUnreadNotificationCountAsync();
    }
}
