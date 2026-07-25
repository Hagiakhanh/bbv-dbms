namespace DBMS.Domain.Catalog.Template;

public abstract class DdlScriptGenerator
{
    public string Generate()
    {
        // var header = BuildHeader();
        // var body = BuildBody();
        // var footer = BuildFooter();

        // if (string.IsNullOrWhiteSpace(body))
        // {
        //     return $"{header}{footer}".TrimEnd();
        // }

        // return $"{header}\n{body}\n{footer}".TrimEnd();
        throw new NotImplementedException();
    }

    protected abstract string BuildHeader();
    protected abstract string BuildBody();
    protected virtual string BuildFooter() => ";";
}
