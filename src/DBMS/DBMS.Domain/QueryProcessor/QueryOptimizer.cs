namespace DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.Transaction;

public class QueryOptimizer
{
    private CostModel costModel;
    private IMetadataCatalog catalog;
    
    public QueryOptimizer(CostModel costModel, IMetadataCatalog catalog)
    {
        this.costModel = costModel;
        this.catalog = catalog;
    }
    private LogicalPlan ToLogicalPlan(ValidatedAst ast)
    {
        throw new System.NotImplementedException();
    }
    private ExecutionPlan ToPhysicalPlan(LogicalPlan plan)
    {
        throw new System.NotImplementedException();
    }
    public ExecutionPlan Optimize(ValidatedAst ast)
    {
        throw new System.NotImplementedException();
    }
}
