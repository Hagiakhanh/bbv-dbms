using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Builder;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Services;

public interface ISchemaService
{
    Table CreateTable(Schema schema, TableDefinition definition);
    void DropTable(Schema schema, string name, bool cascade);
    void RenameTable(Schema schema, string oldName, string newName);
    void AddColumn(Table table, Column column);
    void DropColumn(Table table, string name);
    void AddConstraint(Table table, Constraint constraint);
    void DropConstraint(Table table, string name);
    View CreateView(Schema schema, string name, string query);
    void DropView(Schema schema, string name);
    StoredProcedure CreateProcedure(Schema schema, string name, string body);
    void DropProcedure(Schema schema, string name);
    Sequence CreateSequence(Schema schema, string name);
    void DropSequence(Schema schema, string name);
}
