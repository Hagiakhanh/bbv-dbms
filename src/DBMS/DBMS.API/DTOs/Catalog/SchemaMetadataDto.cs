namespace DBMS.API.DTOs.Catalog
{
    public class SchemaMetadataDto
    {
        public string Name { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public int TableCount { get; set; }
        public List<TableSummaryDto> Tables { get; set; } = new();
    }

    public class TableSummaryDto
    {
        public int TableId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ColumnCount { get; set; }
    }
}
