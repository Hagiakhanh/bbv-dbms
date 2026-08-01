namespace OnlineStore.Entities
{
    public class Store
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Plan { get; set; } = "Basic"; // e.g. Free, Basic, Pro, Enterprise
        public bool LiveStatus { get; set; } = true;
        public string StorefrontUrl { get; set; } = string.Empty;
    }
}
