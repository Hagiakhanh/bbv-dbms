using OnlineStore.Entities;

namespace OnlineStore.Repositories
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

        public Task<Store?> GetStoreAsync()
        {
            var store = _jsonContext.ReadObject<Store>(StoreFileName);
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
