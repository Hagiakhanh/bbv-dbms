using System;
using DBMS.Domain.Catalog;

namespace DBMS.Domain.Query;

public class QueryOptimizer
{
    private object costModel;
    private ICatalogManager catalog;

    public PhysicalPlan Optimize(LogicalPlan plan)
    {
        throw new NotImplementedException();
    }
}
