using DBMS.API.DTOs.Columns;
using DBMS.API.DTOs.Constraints;
using DBMS.API.DTOs.Indexes;

namespace DBMS.API.DTOs.Catalog
{
    public class TableMetadataDto
    {
        public int TableId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SchemaName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public List<ColumnDto> Columns { get; set; } = new();
        public List<ConstraintDto> Constraints { get; set; } = new();
        public List<IndexDto> Indexes { get; set; } = new();
    }
}
