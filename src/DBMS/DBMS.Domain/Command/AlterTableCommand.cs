using System;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Template;
using DBMS.Domain.Services;

namespace DBMS.Domain.Command;

public class AlterTableCommand : IDdlCommand
{
    private readonly ISchemaService _receiver;
    private readonly Table _table;
    private readonly TableAlterOperation _operation;

    public AlterTableCommand(ISchemaService receiver, Table table, TableAlterOperation operation)
    {
        _receiver = receiver;
        _table = table;
        _operation = operation;
    }

    public DdlResult Execute()
    {
        throw new NotImplementedException();
    }
}
