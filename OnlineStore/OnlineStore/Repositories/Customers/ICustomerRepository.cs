using OnlineStore.Entities;

namespace OnlineStore.Repositories.Customers
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(string id);
        Task<int> BulkUpdateStatusAsync(List<string> ids, string status);
        Task<int> BulkDeleteAsync(List<string> ids);
    }
}
