namespace DBMS.Domain.Command;

public interface IDdlCommandExecutor
{
    DdlResult Execute(IDdlCommand command);
}
