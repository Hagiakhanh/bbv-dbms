using OnlineStore.DTOs.Common;
using OnlineStore.DTOs.Customers;

namespace OnlineStore.Services.Customers
{
    public interface ICustomerService
    {
        Task<CustomerSummaryResponse> GetSummaryAsync(DateTime? from, DateTime? to, string? timezone);
        Task<PagedResult<CustomerListItemResponse>> GetPagedAsync(string? search, string? status, string? sort, int page, int pageSize);
        Task<CustomerDetailResponse?> GetByIdAsync(string id);
        Task<CustomerDetailResponse> CreateAsync(CreateCustomerRequest request);
        Task<CustomerDetailResponse?> UpdateAsync(string id, UpdateCustomerRequest request);
        Task<CustomerDetailResponse?> UpdateStatusAsync(string id, UpdateCustomerStatusRequest request);
        Task<MessageResponse> DeleteAsync(string id);
        Task<byte[]> ExportCsvAsync(string? scope, string? status, string? search, string? sort);
        Task<BulkOperationResponse> BulkUpdateStatusAsync(BulkUpdateCustomerStatusRequest request);
        Task<BulkOperationResponse> BulkDeleteAsync(BulkDeleteCustomersRequest request);
        Task<byte[]> BulkExportCsvAsync(BulkCustomerIdsRequest request);
    }
}
