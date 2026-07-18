using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class PhysicalPlan
{
    public List<object> Operators { get; set; }
}
