namespace DBMS.Domain.Command;

public interface IDdlCommand
{
    DdlResult Execute();
}
