using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query.Interpreter;

public class AST
{
    public ASTNode? Root { get; set; }

    public AST() { }

    public AST(ASTNode root)
    {
        Root = root;
    }
}
