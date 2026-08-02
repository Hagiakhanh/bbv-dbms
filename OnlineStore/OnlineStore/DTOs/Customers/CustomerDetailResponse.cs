namespace OnlineStore.DTOs.Customers
{
    public class CustomerDetailResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActiveNow { get; set; }
        public decimal TotalSpent { get; set; }
        public int OrdersCount { get; set; }
        public string Location { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
