using System;
using DBMS.Domain.Services;

namespace DBMS.Domain.QueryProcessing.Commands;

public class DropTableCommand : IDdlCommand
{
    private readonly ISchemaService _receiver;
    private readonly Schema _schema;
    private readonly string _tableName;
    private readonly bool _cascade;

    public DropTableCommand(ISchemaService receiver, Schema schema, string tableName, bool cascade = false)
    {
        _receiver = receiver;
        _schema = schema;
        _tableName = tableName;
        _cascade = cascade;
    }

    public DdlResult Execute()
    {
        throw new NotImplementedException();
    }
}
