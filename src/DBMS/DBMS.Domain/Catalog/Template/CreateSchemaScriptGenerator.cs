using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Template;

public class CreateSchemaScriptGenerator : DdlScriptGenerator
{
    private readonly Schema _schema;

    public CreateSchemaScriptGenerator(Schema schema)
    {
        _schema = schema;
    }

    protected override string BuildHeader()
    {
        // return $"CREATE SCHEMA {_schema.Name}";
        throw new NotImplementedException();
    }

    protected override string BuildBody()
    {
        // return string.Empty;
        throw new NotImplementedException();
    }
}
