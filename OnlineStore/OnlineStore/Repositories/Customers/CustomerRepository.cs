using OnlineStore.Entities;
using OnlineStore.Repositories.Context;

namespace OnlineStore.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly JsonFileContext _jsonContext;
        private const string CustomersFileName = "customers.json";

        public CustomerRepository(JsonFileContext jsonContext)
        {
            _jsonContext = jsonContext;
        }

        public Task<List<Customer>> GetAllAsync()
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            return Task.FromResult(customers);
        }

        public Task<Customer?> GetByIdAsync(string id)
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            var customer = customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }

        public Task AddAsync(Customer customer)
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            customers.Add(customer);
            _jsonContext.WriteList(CustomersFileName, customers);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Customer customer)
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            var index = customers.FindIndex(c => c.Id == customer.Id);
            if (index >= 0)
            {
                customers[index] = customer;
                _jsonContext.WriteList(CustomersFileName, customers);
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            var index = customers.FindIndex(c => c.Id == id);
            if (index >= 0)
            {
                customers.RemoveAt(index);
                _jsonContext.WriteList(CustomersFileName, customers);
            }
            return Task.CompletedTask;
        }

        public Task<int> BulkUpdateStatusAsync(List<string> ids, string status)
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            int count = 0;
            foreach (var customer in customers.Where(c => ids.Contains(c.Id)))
            {
                customer.Status = status;
                count++;
            }

            if (count > 0)
            {
                _jsonContext.WriteList(CustomersFileName, customers);
            }

            return Task.FromResult(count);
        }

        public Task<int> BulkDeleteAsync(List<string> ids)
        {
            var customers = _jsonContext.ReadList<Customer>(CustomersFileName);
            int initialCount = customers.Count;
            customers.RemoveAll(c => ids.Contains(c.Id));
            int removedCount = initialCount - customers.Count;

            if (removedCount > 0)
            {
                _jsonContext.WriteList(CustomersFileName, customers);
            }

            return Task.FromResult(removedCount);
        }
    }
}
