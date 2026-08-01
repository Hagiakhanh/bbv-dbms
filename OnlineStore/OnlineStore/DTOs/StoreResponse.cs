namespace OnlineStore.DTOs
{
    public class StoreResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public bool LiveStatus { get; set; }
        public string StorefrontUrl { get; set; } = string.Empty;
    }
}
