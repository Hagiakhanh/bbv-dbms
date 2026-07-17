namespace DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.Transaction;

public class ExecutionEngine
{
    private IAccessMethod scanner;
    private IAuthorizationManager authz;
    
    public ExecutionEngine(IAccessMethod scanner, IAuthorizationManager authz)
    {
        this.scanner = scanner;
        this.authz = authz;
    }
    private void OpenOperators(ExecutionPlan plan)
    {
        throw new System.NotImplementedException();
    }
    private void CloseOperators()
    {
        throw new System.NotImplementedException();
    }
    public ResultCursor Execute(ExecutionPlan plan, TransactionContext ctx)
    {
        throw new System.NotImplementedException();
    }
}
