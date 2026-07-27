using System;

namespace DBMS.Domain.Server;

public class DatabaseCommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Database? Database { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}
