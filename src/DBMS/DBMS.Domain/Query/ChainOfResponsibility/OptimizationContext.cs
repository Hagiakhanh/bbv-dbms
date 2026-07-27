using System;
using DBMS.Domain.Catalog;

namespace DBMS.Domain.Query.ChainOfResponsibility;

public class OptimizationContext
{
    public ICatalogManager Catalog { get; set; } = null!;
    public StatisticsManager Statistics { get; set; } = null!;
    public CostModel CostModel { get; set; } = null!;
    public int MaxPasses { get; set; } = 10;
}
