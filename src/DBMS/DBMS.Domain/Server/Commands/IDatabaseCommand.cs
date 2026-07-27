namespace DBMS.Domain.Server;

public interface IDatabaseCommand
{
    DatabaseCommandResult Execute();
}
