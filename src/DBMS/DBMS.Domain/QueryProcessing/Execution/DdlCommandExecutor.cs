using System;

namespace DBMS.Domain.QueryProcessing.Execution;

public class DdlCommandExecutor : IDdlCommandExecutor
{
    public DdlResult Execute(IDdlCommand command)
    {
        // if (command == null) throw new ArgumentNullException(nameof(command));
        // return command.Execute();
        throw new NotImplementedException();
    }
}
