using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Query.ChainOfResponsibility;

namespace DBMS.Domain.Query;

public class QueryOptimizer
{
    private readonly OptimizationRulePipeline _rulePipeline;
    private readonly PhysicalPlanGenerator _physicalPlanGenerator;
    private readonly StatisticsManager _statisticsManager;
    private readonly CostModel _costModel;

    public QueryOptimizer()
    {
        _rulePipeline = new OptimizationRulePipeline();
        _physicalPlanGenerator = new PhysicalPlanGenerator();
        _statisticsManager = new StatisticsManager();
        _costModel = new CostModel();
    }

    public QueryOptimizer(
        OptimizationRulePipeline rulePipeline,
        PhysicalPlanGenerator physicalPlanGenerator,
        StatisticsManager statisticsManager,
        CostModel costModel)
    {
        _rulePipeline = rulePipeline;
        _physicalPlanGenerator = physicalPlanGenerator;
        _statisticsManager = statisticsManager;
        _costModel = costModel;
    }

    public PhysicalPlan Optimize(LogicalPlan plan)
    {
        throw new NotImplementedException();
    }
}
