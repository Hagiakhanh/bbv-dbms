namespace DBMS.Domain.Catalog.ScriptGenerators;

public class DropTableScriptGenerator : DdlScriptGenerator
{
    private readonly string _tableName;
    private readonly bool _cascade;

    public DropTableScriptGenerator(string tableName, bool cascade = false)
    {
        _tableName = tableName;
        _cascade = cascade;
    }

    protected override string BuildHeader()
    {
        // return _cascade
        //     ? $"DROP TABLE IF EXISTS {_tableName} CASCADE"
        //     : $"DROP TABLE IF EXISTS {_tableName}";
        throw new NotImplementedException();
    }

    protected override string BuildBody()
    {
        // return string.Empty;
        throw new NotImplementedException();
    }
}
