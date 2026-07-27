using System;

namespace DBMS.Domain.Server;

public class DropDatabaseCommand : IDatabaseCommand
{
    private readonly DatabaseManager _databaseManager;
    private readonly string _databaseName;
    private readonly bool _cascade;

    public DropDatabaseCommand(DatabaseManager databaseManager, string databaseName, bool cascade = false)
    {
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        _cascade = cascade;
    }

    public DatabaseCommandResult Execute()
    {
        throw new NotImplementedException();
    }
}
