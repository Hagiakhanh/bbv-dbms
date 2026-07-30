namespace DBMS.Domain.Server.Commands;

public interface IDatabaseCommand
{
    DatabaseCommandResult Execute();
}
