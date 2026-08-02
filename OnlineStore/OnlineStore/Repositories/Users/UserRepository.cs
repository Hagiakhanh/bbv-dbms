using OnlineStore.Entities;
using OnlineStore.Repositories.Context;

namespace OnlineStore.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly JsonFileContext _jsonContext;
        private const string UsersFileName = "users.json";

        public UserRepository(JsonFileContext jsonContext)
        {
            _jsonContext = jsonContext;
        }

        public Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            var users = _jsonContext.ReadList<User>(UsersFileName);
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
                u.Username.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(string id)
        {
            var users = _jsonContext.ReadList<User>(UsersFileName);
            var user = users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task UpdateAsync(User user)
        {
            var users = _jsonContext.ReadList<User>(UsersFileName);
            var index = users.FindIndex(u => u.Id == user.Id);
            if (index >= 0)
            {
                users[index] = user;
                _jsonContext.WriteList(UsersFileName, users);
            }
            return Task.CompletedTask;
        }
    }
}
