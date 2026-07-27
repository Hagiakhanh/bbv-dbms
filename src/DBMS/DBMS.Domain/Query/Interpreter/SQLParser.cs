using System;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.Query.Interpreter;

public class SQLParser
{
    private readonly Lexer _lexer = new();

    public AST Parse(string sql)
    {
        throw new NotImplementedException();
    }

    private Token[] Tokenize(string sql)
    {
        throw new NotImplementedException();
    }

    private ASTNode BuildAST(Token[] tokens)
    {
        throw new NotImplementedException();
    }

    private SelectNode ParseSelect(Token[] tokens)
    {
        throw new NotImplementedException();
    }

    private ASTNode ParseExpression(Token[] tokens)
    {
        throw new NotImplementedException();
    }
}
