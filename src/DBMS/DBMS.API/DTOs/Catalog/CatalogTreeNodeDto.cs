namespace DBMS.API.DTOs.Catalog
{
    public class CatalogTreeNodeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NodeType { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public List<CatalogTreeNodeDto> Children { get; set; } = new();
    }
}
