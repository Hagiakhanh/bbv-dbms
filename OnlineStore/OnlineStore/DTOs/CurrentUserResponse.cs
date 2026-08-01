namespace OnlineStore.DTOs
{
    public class CurrentUserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
        public StoreResponse? Store { get; set; }
    }
}
