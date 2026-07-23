using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class QueryOptimizer
{
    private object costModel;
    private ICatalogManager catalog;

    public PhysicalPlan Optimize(object ast)
    {
        throw new NotImplementedException();
    }
}
