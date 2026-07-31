namespace DBMS.Application.DTOs
{
    public class CreateDatabaseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = "sa";
    }
}
