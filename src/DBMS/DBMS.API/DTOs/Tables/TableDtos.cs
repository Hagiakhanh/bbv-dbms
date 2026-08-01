namespace DBMS.API.DTOs.Tables
{
    public class CreateTableRequest
    {
        public string Name { get; set; } = string.Empty;
        public string SchemaName { get; set; } = "dbo";
        public string DatabaseName { get; set; } = "master";
    }

    public class UpdateTableRequest
    {
        public string? NewName { get; set; }
    }

    public class TableDto
    {
        public int TableId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SchemaName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public int ColumnCount { get; set; }
        public int ConstraintCount { get; set; }
        public int IndexCount { get; set; }
    }
}
