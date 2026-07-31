namespace DBMS.API.DTOs.Schemas
{
    public class CreateSchemaRequest
    {
        public string DatabaseName { get; set; } = "master";
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = "dbo";
    }

    public class RenameSchemaRequest
    {
        public string NewName { get; set; } = string.Empty;
    }

    public class SchemaDto
    {
        public int SchemaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public int TableCount { get; set; }
    }
}
