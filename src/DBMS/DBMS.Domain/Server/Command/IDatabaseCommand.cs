namespace DBMS.Domain.Server.Command;

public interface IDatabaseCommand
{
    DatabaseCommandResult Execute();
}
