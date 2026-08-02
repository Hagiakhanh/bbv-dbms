using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DTOs.Customers
{
    public class BulkUpdateCustomerStatusRequest
    {
        [Required]
        public List<string> CustomerIds { get; set; } = new List<string>();

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
