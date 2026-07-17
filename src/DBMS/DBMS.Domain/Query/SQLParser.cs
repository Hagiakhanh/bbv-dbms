using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class SQLParser
{
    public ASTNode Parse(sql : string) 
    { 
        throw new NotImplementedException(); 
    }

    private Token[] Tokenize(sql : string) 
    { 
        throw new NotImplementedException(); 
    }
    
    private ASTNode BuildAST(tokens : Token[]) 
    { 
        throw new NotImplementedException(); 
    }
}
