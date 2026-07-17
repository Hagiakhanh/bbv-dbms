namespace DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.Transaction;

public class SqlParser
{
    private string sqlText;
    private Token[] Tokenize()
    {
        throw new System.NotImplementedException();
    }
    private ASTNode BuildAST(Token[] tokens)
    {
        throw new System.NotImplementedException();
    }
    public ASTNode Parse(string sql)
    {
        throw new System.NotImplementedException();
    }
}
