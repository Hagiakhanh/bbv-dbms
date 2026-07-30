using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Exceptions;

namespace DBMS.Domain.QueryProcessing.Parsing;

public class SemanticAnalyzer
{
    private readonly ICatalogManager _catalog;

    public SemanticAnalyzer(ICatalogManager catalog)
    {
        _catalog = catalog;
    }

    public LogicalPlan Bind(ASTNode ast)
    {
        throw new NotImplementedException();
    }

    public LogicalPlan Bind(AST ast)
    {
        if (ast?.Root == null)
        {
            throw new ArgumentNullException(nameof(ast));
        }
        return Bind(ast.Root);
    }

    private Column ResolveIdentifier(IdentifierNode node)
    {
        throw new NotImplementedException();
    }

    private ASTNode BindExpression(ASTNode node)
    {
        throw new NotImplementedException();
    }
}
