using System;

namespace DBMS.Domain.Server.Commands;

public class RenameDatabaseCommand : IDatabaseCommand
{
    private readonly DatabaseManager _databaseManager;
    private readonly string _oldName;
    private readonly string _newName;

    public RenameDatabaseCommand(DatabaseManager databaseManager, string oldName, string newName)
    {
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _oldName = oldName ?? throw new ArgumentNullException(nameof(oldName));
        _newName = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public DatabaseCommandResult Execute()
    {
        throw new NotImplementedException();
    }
}
