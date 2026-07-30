using System;
using DBMS.Domain.Services;

namespace DBMS.Domain.QueryProcessing.Commands;

public class CreateTableCommand : IDdlCommand
{
    private readonly ISchemaService _receiver;
    private readonly Schema _schema;
    private readonly TableDefinition _definition;

    public CreateTableCommand(ISchemaService receiver, Schema schema, TableDefinition definition)
    {
        _receiver = receiver;
        _schema = schema;
        _definition = definition;
    }

    public DdlResult Execute()
    {
        throw new NotImplementedException();
    }
}
