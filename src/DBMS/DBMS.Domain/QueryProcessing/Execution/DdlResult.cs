
namespace DBMS.Domain.QueryProcessing.Execution;

public class DdlResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ICatalogComponent AffectedObject { get; set; }
}
