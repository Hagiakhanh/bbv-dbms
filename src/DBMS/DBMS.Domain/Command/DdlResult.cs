using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Command;

public class DdlResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ICatalogComponent AffectedObject { get; set; }
}
