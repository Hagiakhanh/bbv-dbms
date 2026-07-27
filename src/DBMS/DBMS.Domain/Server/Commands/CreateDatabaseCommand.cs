using System;

namespace DBMS.Domain.Server;

public class CreateDatabaseCommand : IDatabaseCommand
{
    private readonly DatabaseManager _databaseManager;
    private readonly string _databaseName;

    public CreateDatabaseCommand(DatabaseManager databaseManager, string databaseName)
    {
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
    }

    public DatabaseCommandResult Execute()
    {
        throw new NotImplementedException();
    }
}
