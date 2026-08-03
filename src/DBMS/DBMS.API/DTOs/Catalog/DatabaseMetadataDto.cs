namespace DBMS.API.DTOs.Catalog
{
    public class DatabaseMetadataDto
    {
        public int DatabaseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int SchemaCount { get; set; }
        public int TableCount { get; set; }
        public List<SchemaSummaryDto> Schemas { get; set; } = new();
    }

    public class SchemaSummaryDto
    {
        public string Name { get; set; } = string.Empty;
        public int TableCount { get; set; }
    }
}
