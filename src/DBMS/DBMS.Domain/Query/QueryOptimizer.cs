using System;
using DBMS.Domain.Catalog;

namespace DBMS.Domain.Query;

public class QueryOptimizer
{
    private readonly CostModel _costModel;
    private readonly ICatalogManager _catalog;

    public QueryOptimizer()
    {
        _costModel = new CostModel();
        _catalog = null!;
    }

    public QueryOptimizer(CostModel costModel, ICatalogManager catalog)
    {
        _costModel = costModel;
        _catalog = catalog;
    }

    public PhysicalPlan Optimize(LogicalPlan plan)
    {
        throw new NotImplementedException();
    }
}
