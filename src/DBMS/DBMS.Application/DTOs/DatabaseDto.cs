namespace DBMS.Application.DTOs
{
    public class DatabaseDto
    {
        public int DatabaseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public int SchemaCount { get; set; }
    }
}
