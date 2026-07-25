namespace DBMS.Domain.Catalog.Factory;

public class DatabaseCreationOptions
{
    public string Name { get; set; } = string.Empty;
    public string Owner { get; set; } = "sa";
    public string Encoding { get; set; } = "UTF-8";
    public string CollationName { get; set; } = "utf8_general_ci";
    public bool IsTemplate { get; set; }
}
