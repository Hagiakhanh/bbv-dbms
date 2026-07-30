using System;
using System.Collections.Generic;
using DBMS.Domain.Transactions;

namespace DBMS.Domain.QueryProcessing.Execution;

public class QueryExecutor
{
    public ResultCursor Execute(PhysicalPlan plan, RuntimeContext ctx)
    {
        throw new NotImplementedException();
    }

    public object Execute(PhysicalPlan plan, Transaction tx)
    {
        throw new NotImplementedException();
    }

    public object ExecuteSelect(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteInsert(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteUpdate(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteDelete(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteJoin(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteAggregate(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteLimit(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }

    public object ExecuteOrderBy(PhysicalPlan plan, Transaction tx)
    {
        return Execute(plan, tx);
    }
}
