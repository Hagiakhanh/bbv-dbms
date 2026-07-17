namespace DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.Transaction;

public class QueryValidator
{
    private IMetadataCatalog catalog;
    private IAuthorizationManager authz;
    
    public QueryValidator(IMetadataCatalog catalog, IAuthorizationManager authz)
    {
        this.catalog = catalog;
        this.authz = authz;
    }
    private void CheckTableExists(ASTNode node)
    {
        throw new System.NotImplementedException();
    }
    private void CheckColumnTypes(ASTNode node)
    {
        throw new System.NotImplementedException();
    }
    public ValidatedAst Validate(ASTNode ast)
    {
        throw new System.NotImplementedException();
    }
}
