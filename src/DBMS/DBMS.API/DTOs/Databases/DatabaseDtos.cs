namespace DBMS.API.DTOs.Databases
{
    public class CreateDatabaseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = "sa";
    }

    public class UpdateDatabaseRequest
    {
        public string? NewName { get; set; }
        public string? NewOwner { get; set; }
    }

    public class DatabaseDto
    {
        public int DatabaseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public int SchemaCount { get; set; }
    }
}
