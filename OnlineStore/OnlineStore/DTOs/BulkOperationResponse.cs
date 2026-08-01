namespace OnlineStore.DTOs
{
    public class BulkOperationResponse
    {
        public bool Success { get; set; } = true;
        public int AffectedCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
