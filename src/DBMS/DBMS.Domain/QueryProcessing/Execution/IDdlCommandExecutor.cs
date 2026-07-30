namespace DBMS.Domain.QueryProcessing.Execution;

public interface IDdlCommandExecutor
{
    DdlResult Execute(IDdlCommand command);
}
