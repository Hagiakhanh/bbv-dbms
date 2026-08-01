using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DTOs
{
    public class BulkDeleteCustomersRequest
    {
        [Required]
        public List<string> CustomerIds { get; set; } = new List<string>();
    }
}
