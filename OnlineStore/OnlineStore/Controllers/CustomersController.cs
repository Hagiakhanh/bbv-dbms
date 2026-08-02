using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.DTOs.Common;
using OnlineStore.DTOs.Customers;
using OnlineStore.Services.Customers;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<CustomerSummaryResponse>> GetSummary(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] string? timezone = null)
        {
            var summary = await _customerService.GetSummaryAsync(from, to, timezone);
            return Ok(summary);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<CustomerListItemResponse>>> GetCustomers(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var paged = await _customerService.GetPagedAsync(search, status, sort, page, pageSize);
            return Ok(paged);
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<CustomerDetailResponse>> GetCustomerById(string customerId)
        {
            var result = await _customerService.GetByIdAsync(customerId);
            if (result == null)
            {
                return NotFound(new { message = "Customer not found." });
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDetailResponse>> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var result = await _customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetCustomerById), new { customerId = result.Id }, result);
        }

        [HttpPut("{customerId}")]
        public async Task<ActionResult<CustomerDetailResponse>> UpdateCustomer(
            string customerId,
            [FromBody] UpdateCustomerRequest request)
        {
            var result = await _customerService.UpdateAsync(customerId, request);
            if (result == null)
            {
                return NotFound(new { message = "Customer not found." });
            }
            return Ok(result);
        }

        [HttpPatch("{customerId}/status")]
        public async Task<ActionResult<CustomerDetailResponse>> UpdateCustomerStatus(
            string customerId,
            [FromBody] UpdateCustomerStatusRequest request)
        {
            var result = await _customerService.UpdateStatusAsync(customerId, request);
            if (result == null)
            {
                return NotFound(new { message = "Customer not found." });
            }
            return Ok(result);
        }

        [HttpDelete("{customerId}")]
        public async Task<ActionResult<MessageResponse>> DeleteCustomer(string customerId)
        {
            var result = await _customerService.DeleteAsync(customerId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportCustomers(
            [FromQuery] string? scope = null,
            [FromQuery] string? format = "csv",
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sort = null)
        {
            var csvBytes = await _customerService.ExportCsvAsync(scope, status, search, sort);
            var fileName = $"customers_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(csvBytes, "text/csv", fileName);
        }

        [HttpPatch("bulk-status")]
        public async Task<ActionResult<BulkOperationResponse>> BulkUpdateStatus(
            [FromBody] BulkUpdateCustomerStatusRequest request,
            [FromQuery] bool notifyCustomers = false)
        {
            var result = await _customerService.BulkUpdateStatusAsync(request);
            return Ok(result);
        }

        [HttpPost("bulk-delete")]
        public async Task<ActionResult<BulkOperationResponse>> BulkDelete([FromBody] BulkDeleteCustomersRequest request)
        {
            var result = await _customerService.BulkDeleteAsync(request);
            return Ok(result);
        }

        [HttpPost("bulk-export")]
        public async Task<IActionResult> BulkExport(
            [FromBody] BulkCustomerIdsRequest request,
            [FromQuery] string? format = "csv")
        {
            var csvBytes = await _customerService.BulkExportCsvAsync(request);
            var fileName = $"customers_bulk_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(csvBytes, "text/csv", fileName);
        }
    }
}
