using System;

namespace DBMS.Domain.Catalog.Events;

public enum MetadataEventType
{
    CREATED,
    UPDATED,
    RENAMED,
    REMOVED
}

public class MetadataChangeContext
{
    public string Actor { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class MetadataEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public MetadataEventType EventType { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public MetadataChangeContext Context { get; set; } = new();
}
