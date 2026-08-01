using System.Collections.Generic;

namespace DBMS.API.DTOs.Indexes
{
    public class CreateIndexRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "BTREE"; // BTREE, HASH, BITMAP
        public List<string> Columns { get; set; } = new();
        public bool IsUnique { get; set; }
    }

    public class IndexDto
    {
        public int IndexId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string SchemaName { get; set; } = "dbo";
        public string DatabaseName { get; set; } = "master";
        public List<string> Columns { get; set; } = new();
        public bool IsUnique { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
