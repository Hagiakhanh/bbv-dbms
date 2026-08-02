using OnlineStore.Entities;

namespace OnlineStore.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);
        Task<User?> GetByIdAsync(string id);
        Task UpdateAsync(User user);
    }
}
