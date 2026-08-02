using OnlineStore.Entities;

namespace OnlineStore.Repositories.Store
{
    public interface IStoreRepository
    {
        Task<Entities.Store?> GetStoreAsync();
        Task<int> GetUnreadNotificationCountAsync();
    }
}
