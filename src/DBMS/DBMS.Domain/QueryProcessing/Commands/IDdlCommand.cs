namespace DBMS.Domain.QueryProcessing.Commands;

public interface IDdlCommand
{
    DdlResult Execute();
}
