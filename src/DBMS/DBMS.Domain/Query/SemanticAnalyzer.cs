using System;
using DBMS.Domain.Catalog;

namespace DBMS.Domain.Query;

public class SemanticAnalyzer
{
    private readonly ICatalogManager _catalog;

    public SemanticAnalyzer(ICatalogManager catalog)
    {
        _catalog = catalog;
    }

    public LogicalPlan Bind(object ast)
    {
        throw new NotImplementedException();
    }
}
