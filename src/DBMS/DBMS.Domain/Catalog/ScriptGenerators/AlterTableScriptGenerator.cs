
namespace DBMS.Domain.Catalog.ScriptGenerators;

public class AlterTableScriptGenerator : DdlScriptGenerator
{
    private readonly Table _table;
    private readonly TableAlterOperation _operation;

    public AlterTableScriptGenerator(Table table, TableAlterOperation operation)
    {
        _table = table;
        _operation = operation;
    }

    protected override string BuildHeader()
    {
        // var schemaName = _table.Parent?.Name;
        // return string.IsNullOrEmpty(schemaName)
        //     ? $"ALTER TABLE {_table.Name}"
        //     : $"ALTER TABLE {schemaName}.{_table.Name}";
        throw new NotImplementedException();
    }

    protected override string BuildBody()
    {
        // return $"    {_operation.Type} {_operation.Definition}".TrimEnd();
        throw new NotImplementedException();
    }
}
