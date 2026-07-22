using System;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Services;

namespace DBMS.Domain.Command;

public class CreateSchemaCommand : IDdlCommand
{
    private readonly IDatabaseService _receiver;
    private readonly Database _database;
    private readonly string _schemaName;

    public CreateSchemaCommand(IDatabaseService receiver, Database database, string schemaName)
    {
        _receiver = receiver;
        _database = database;
        _schemaName = schemaName;
    }

    public DdlResult Execute()
    {
        throw new NotImplementedException();
    }
}
