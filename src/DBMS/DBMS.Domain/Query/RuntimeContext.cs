using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class RuntimeContext
{
    public int TransactionId { get; set; }
    public string SessionId { get; set; }
}
