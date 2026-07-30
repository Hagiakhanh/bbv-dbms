using System;

namespace DBMS.Domain.QueryProcessing.Models;

public enum IsolationLevel
{
    ReadUncommitted,
    ReadCommitted,
    RepeatableRead,
    Serializable
}

public class RuntimeContext
{
    public int TransactionId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
}
