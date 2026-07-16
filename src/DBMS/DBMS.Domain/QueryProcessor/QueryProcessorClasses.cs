namespace DBMS.Domain.QueryProcessor;

using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.Transaction;

public class SqlParser
{
    private string sqlText;
    private Token[] Tokenize() => throw new System.NotImplementedException();
    private ASTNode BuildAST(Token[] tokens) => throw new System.NotImplementedException();
    public ASTNode Parse(string sql) => throw new System.NotImplementedException();
}

public class QueryValidator
{
    private IMetadataCatalog catalog;
    private IAuthorizationManager authz;
    
    public QueryValidator(IMetadataCatalog catalog, IAuthorizationManager authz)
    {
        this.catalog = catalog;
        this.authz = authz;
    }
    private void CheckTableExists(ASTNode node) => throw new System.NotImplementedException();
    private void CheckColumnTypes(ASTNode node) => throw new System.NotImplementedException();
    public ValidatedAst Validate(ASTNode ast) => throw new System.NotImplementedException();
}

public class QueryOptimizer
{
    private CostModel costModel;
    private IMetadataCatalog catalog;
    
    public QueryOptimizer(CostModel costModel, IMetadataCatalog catalog)
    {
        this.costModel = costModel;
        this.catalog = catalog;
    }
    private LogicalPlan ToLogicalPlan(ValidatedAst ast) => throw new System.NotImplementedException();
    private ExecutionPlan ToPhysicalPlan(LogicalPlan plan) => throw new System.NotImplementedException();
    public ExecutionPlan Optimize(ValidatedAst ast) => throw new System.NotImplementedException();
}

public class ExecutionEngine
{
    private IAccessMethod scanner;
    private IAuthorizationManager authz;
    
    public ExecutionEngine(IAccessMethod scanner, IAuthorizationManager authz)
    {
        this.scanner = scanner;
        this.authz = authz;
    }
    private void OpenOperators(ExecutionPlan plan) => throw new System.NotImplementedException();
    private void CloseOperators() => throw new System.NotImplementedException();
    public ResultCursor Execute(ExecutionPlan plan, TransactionContext ctx) => throw new System.NotImplementedException();
}

public class ResultProcessor
{
    public ResultSet Format(ResultCursor cursor) => throw new System.NotImplementedException();
    public byte[] Serialize(Tuple row) => throw new System.NotImplementedException();
    public Page<Tuple> Paginate(ResultCursor cursor, int pageSize) => throw new System.NotImplementedException();
}
