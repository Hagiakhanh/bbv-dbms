using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DTOs
{
    public class UpdateCustomerStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
