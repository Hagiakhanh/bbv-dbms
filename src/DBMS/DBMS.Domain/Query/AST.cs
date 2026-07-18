using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class AST
{
    public object Root { get; set; }

    public LogicalPlan ToLogicalPlan()
    {
        throw new NotImplementedException();
    }
}
