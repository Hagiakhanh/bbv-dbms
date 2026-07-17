using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class AST
{
    public ASTNode Root { get; set; }
    public LogicalPlan ToLogicalPlan() 
    { 
        throw new NotImplementedException(); 
    }
}
