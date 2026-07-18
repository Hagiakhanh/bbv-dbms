using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class LogicalPlan
{
    public List<object> Operators { get; set; }
}
