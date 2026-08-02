using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DTOs.Customers
{
    public class BulkCustomerIdsRequest
    {
        [Required]
        public List<string> CustomerIds { get; set; } = new List<string>();
    }
}
