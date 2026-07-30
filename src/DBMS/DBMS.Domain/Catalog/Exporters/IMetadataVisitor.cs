using System;

namespace DBMS.Domain.Catalog.Exporters;

public interface IMetadataVisitor
{
    void VisitDatabase(Database database);
    void VisitSchema(Schema schema);
    void VisitTable(Table table);
    void VisitColumn(Column column);
    void VisitConstraint(Constraint constraint);
    void VisitIndex(Index index);
    void VisitPartition(Partition partition);
    void VisitTrigger(Trigger trigger);
    void VisitView(View view);
    void VisitStoredProcedure(StoredProcedure procedure);
    void VisitSequence(Sequence sequence);
}
