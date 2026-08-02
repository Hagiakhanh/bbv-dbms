using System.Text;
using OnlineStore.DTOs.Common;
using OnlineStore.DTOs.Customers;
using OnlineStore.Entities;
using OnlineStore.Repositories.Customers;

namespace OnlineStore.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerSummaryResponse> GetSummaryAsync(DateTime? from, DateTime? to, string? timezone)
        {
            var customers = await _customerRepository.GetAllAsync();

            int totalCustomers = customers.Count;
            int membersCount = customers.Count(c => c.Status.Equals("Member", StringComparison.OrdinalIgnoreCase));
            int activeNowCount = customers.Count(c => c.IsActiveNow);

            return new CustomerSummaryResponse
            {
                TotalCustomers = totalCustomers,
                TotalCustomersGrowthPercentage = 12.5,
                MembersCount = membersCount,
                MembersGrowthPercentage = 8.3,
                ActiveNowCount = activeNowCount,
                ActiveNowGrowthPercentage = 15.0
            };
        }

        public async Task<PagedResult<CustomerListItemResponse>> GetPagedAsync(
            string? search,
            string? status,
            string? sort,
            int page,
            int pageSize)
        {
            var customers = await _customerRepository.GetAllAsync();
            var query = customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchLower) ||
                    c.Email.ToLower().Contains(searchLower) ||
                    c.Phone.Contains(searchLower));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            query = sort?.ToLower() switch
            {
                "name_asc" => query.OrderBy(c => c.Name),
                "name_desc" => query.OrderByDescending(c => c.Name),
                "date_asc" => query.OrderBy(c => c.CreatedAt),
                "date_desc" => query.OrderByDescending(c => c.CreatedAt),
                "spent_asc" => query.OrderBy(c => c.TotalSpent),
                "spent_desc" => query.OrderByDescending(c => c.TotalSpent),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            int totalItems = query.Count();
            int currentPage = page > 0 ? page : 1;
            int currentPageSize = pageSize > 0 ? pageSize : 10;

            var items = query
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .Select(c => MapToListItemDto(c))
                .ToList();

            return new PagedResult<CustomerListItemResponse>
            {
                Items = items,
                TotalItems = totalItems,
                Page = currentPage,
                PageSize = currentPageSize
            };
        }

        public async Task<CustomerDetailResponse?> GetByIdAsync(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            return MapToDetailDto(customer);
        }

        public async Task<CustomerDetailResponse> CreateAsync(CreateCustomerRequest request)
        {
            var customer = new Customer
            {
                Id = "cust-" + Guid.NewGuid().ToString("N")[..8],
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "Customer" : request.Status,
                Location = request.Location,
                AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl)
                    ? $"https://api.dicebear.com/7.x/avataaars/svg?seed={Uri.EscapeDataString(request.Name)}"
                    : request.AvatarUrl,
                CreatedAt = DateTime.UtcNow,
                IsActiveNow = true,
                TotalSpent = 0,
                OrdersCount = 0
            };

            await _customerRepository.AddAsync(customer);
            return MapToDetailDto(customer);
        }

        public async Task<CustomerDetailResponse?> UpdateAsync(string id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.Status = request.Status;
            customer.Location = request.Location;
            if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
            {
                customer.AvatarUrl = request.AvatarUrl;
            }

            await _customerRepository.UpdateAsync(customer);
            return MapToDetailDto(customer);
        }

        public async Task<CustomerDetailResponse?> UpdateStatusAsync(string id, UpdateCustomerStatusRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            customer.Status = request.Status;
            await _customerRepository.UpdateAsync(customer);
            return MapToDetailDto(customer);
        }

        public async Task<MessageResponse> DeleteAsync(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return new MessageResponse
                {
                    Success = false,
                    Message = "Customer not found."
                };
            }

            await _customerRepository.DeleteAsync(id);
            return new MessageResponse
            {
                Success = true,
                Message = "Customer deleted successfully."
            };
        }

        public async Task<byte[]> ExportCsvAsync(string? scope, string? status, string? search, string? sort)
        {
            var pagedResult = await GetPagedAsync(search, status, sort, page: 1, pageSize: 10000);
            return GenerateCsvBytes(pagedResult.Items);
        }

        public async Task<BulkOperationResponse> BulkUpdateStatusAsync(BulkUpdateCustomerStatusRequest request)
        {
            if (request.CustomerIds == null || request.CustomerIds.Count == 0)
            {
                return new BulkOperationResponse
                {
                    Success = false,
                    AffectedCount = 0,
                    Message = "No customer IDs provided."
                };
            }

            int count = await _customerRepository.BulkUpdateStatusAsync(request.CustomerIds, request.Status);
            return new BulkOperationResponse
            {
                Success = true,
                AffectedCount = count,
                Message = $"Updated status for {count} customer(s)."
            };
        }

        public async Task<BulkOperationResponse> BulkDeleteAsync(BulkDeleteCustomersRequest request)
        {
            if (request.CustomerIds == null || request.CustomerIds.Count == 0)
            {
                return new BulkOperationResponse
                {
                    Success = false,
                    AffectedCount = 0,
                    Message = "No customer IDs provided."
                };
            }

            int count = await _customerRepository.BulkDeleteAsync(request.CustomerIds);
            return new BulkOperationResponse
            {
                Success = true,
                AffectedCount = count,
                Message = $"Deleted {count} customer(s)."
            };
        }

        public async Task<byte[]> BulkExportCsvAsync(BulkCustomerIdsRequest request)
        {
            var customers = await _customerRepository.GetAllAsync();
            var filtered = customers
                .Where(c => request.CustomerIds.Contains(c.Id))
                .Select(c => MapToListItemDto(c))
                .ToList();

            return GenerateCsvBytes(filtered);
        }

        private static CustomerListItemResponse MapToListItemDto(Customer c)
        {
            return new CustomerListItemResponse
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Status = c.Status,
                IsActiveNow = c.IsActiveNow,
                TotalSpent = c.TotalSpent,
                OrdersCount = c.OrdersCount,
                Location = c.Location,
                AvatarUrl = c.AvatarUrl,
                CreatedAt = c.CreatedAt
            };
        }

        private static CustomerDetailResponse MapToDetailDto(Customer c)
        {
            return new CustomerDetailResponse
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Status = c.Status,
                IsActiveNow = c.IsActiveNow,
                TotalSpent = c.TotalSpent,
                OrdersCount = c.OrdersCount,
                Location = c.Location,
                AvatarUrl = c.AvatarUrl,
                CreatedAt = c.CreatedAt
            };
        }

        private static byte[] GenerateCsvBytes(List<CustomerListItemResponse> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Name,Email,Phone,Status,IsActiveNow,TotalSpent,OrdersCount,Location,CreatedAt");

            foreach (var item in items)
            {
                sb.AppendLine($"\"{item.Id}\",\"{item.Name}\",\"{item.Email}\",\"{item.Phone}\",\"{item.Status}\",{item.IsActiveNow},{item.TotalSpent},{item.OrdersCount},\"{item.Location}\",\"{item.CreatedAt:o}\"");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
