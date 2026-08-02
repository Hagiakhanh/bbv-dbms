using OnlineStore.Entities;
using OnlineStore.Repositories.Context;

namespace OnlineStore.Repositories.Store
{
    public class StoreRepository : IStoreRepository
    {
        private readonly JsonFileContext _jsonContext;
        private const string StoreFileName = "store.json";
        private const string NotificationsFileName = "notifications.json";

        public StoreRepository(JsonFileContext jsonContext)
        {
            _jsonContext = jsonContext;
        }

        public Task<Entities.Store?> GetStoreAsync()
        {
            var store = _jsonContext.ReadObject<Entities.Store>(StoreFileName);
            return Task.FromResult(store);
        }

        public Task<int> GetUnreadNotificationCountAsync()
        {
            var notifications = _jsonContext.ReadList<UpdateNotification>(NotificationsFileName);
            var unreadCount = notifications.Count(n => !n.IsRead);
            return Task.FromResult(unreadCount);
        }
    }
}
