using System;
using System.Text;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Domain.Catalog.Visitor;

public class DdlExportVisitor : IMetadataVisitor
{
    private readonly StringBuilder _ddl = new();

    public void VisitDatabase(Database database) { throw new NotImplementedException(); }
    public void VisitSchema(Schema schema) { throw new NotImplementedException(); }
    public void VisitTable(Table table) { throw new NotImplementedException(); }
    public void VisitColumn(Column column) { throw new NotImplementedException(); }
    public void VisitConstraint(Constraint constraint) { throw new NotImplementedException(); }
    public void VisitIndex(Index index) { throw new NotImplementedException(); }
    public void VisitPartition(Partition partition) { throw new NotImplementedException(); }
    public void VisitTrigger(Trigger trigger) { throw new NotImplementedException(); }
    public void VisitView(View view) { throw new NotImplementedException(); }
    public void VisitStoredProcedure(StoredProcedure procedure) { throw new NotImplementedException(); }
    public void VisitSequence(Sequence sequence) { throw new NotImplementedException(); }

    public string GetResult() => _ddl.ToString();
}
