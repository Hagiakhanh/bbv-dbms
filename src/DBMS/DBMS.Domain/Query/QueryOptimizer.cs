using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class QueryOptimizer
{
    private CostModel costModel;
    private CatalogManager catalog;
    public PhysicalPlan Optimize(ast : ASTNode) 
    { 
        throw new NotImplementedException(); 
    }
}
