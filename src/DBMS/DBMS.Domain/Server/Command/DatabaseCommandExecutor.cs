using System;
using System.Collections.Generic;

namespace DBMS.Domain.Server.Command;

public class DatabaseCommandExecutor
{
    private readonly List<IDatabaseCommand> _history = new();

    public DatabaseCommandResult Execute(IDatabaseCommand command)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<IDatabaseCommand> GetHistory()
    {
        throw new NotImplementedException();
    }
}
