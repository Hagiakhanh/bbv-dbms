using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class AST
{
    public ASTNode? Root { get; set; }

    public AST() { }

    public AST(ASTNode root)
    {
        Root = root;
    }

    public LogicalPlan ToLogicalPlan()
    {
        throw new NotImplementedException();
    }
}
