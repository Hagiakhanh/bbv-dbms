using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DTOs
{
    public class CreateCustomerRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = "Customer"; // Customer, Member, Churned
        public string Location { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
