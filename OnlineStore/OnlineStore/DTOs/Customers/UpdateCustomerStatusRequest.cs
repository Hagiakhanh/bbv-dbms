using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DTOs.Customers
{
    public class UpdateCustomerStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
