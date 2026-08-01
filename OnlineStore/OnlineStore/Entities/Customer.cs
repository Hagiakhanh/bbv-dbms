namespace OnlineStore.Entities
{
    public class Customer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = "Customer"; // e.g., Customer, Member, Churned
        public bool IsActiveNow { get; set; } = false;
        public decimal TotalSpent { get; set; } = 0;
        public int OrdersCount { get; set; } = 0;
        public string Location { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
