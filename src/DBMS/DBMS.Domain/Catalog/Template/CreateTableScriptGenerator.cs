using System.Collections.Generic;
using System.Linq;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Catalog.Template;

public class CreateTableScriptGenerator : DdlScriptGenerator
{
    private readonly Table _table;

    public CreateTableScriptGenerator(Table table)
    {
        _table = table;
    }

    protected override string BuildHeader()
    {
        // var schemaName = _table.Parent?.Name;
        // return string.IsNullOrEmpty(schemaName)
        //     ? $"CREATE TABLE {_table.Name} ("
        //     : $"CREATE TABLE {schemaName}.{_table.Name} (";
        throw new NotImplementedException();
    }

    protected override string BuildBody()
    {
        // var parts = new List<string>();

        // if (_table.Columns != null)
        // {
        //     foreach (var col in _table.Columns)
        //     {
        //         var nullability = col.Nullable ? "" : " NOT NULL";
        //         var defaultVal = col.DefaultValue != null ? $" DEFAULT {col.DefaultValue}" : "";
        //         parts.Add($"    {col.Name} {col.DataType}{nullability}{defaultVal}");
        //     }
        // }

        // if (_table.Constraints != null)
        // {
        //     foreach (var constraint in _table.Constraints)
        //     {
        //         parts.Add($"    CONSTRAINT {constraint.Name}");
        //     }
        // }

        // return string.Join(",\n", parts);
        throw new NotImplementedException();
    }

    protected override string BuildFooter()
    {
        // return ");";
        throw new NotImplementedException();
    }
}
