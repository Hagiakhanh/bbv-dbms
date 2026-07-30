using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Exporters;

public enum MetadataDependencyType
{
    CONTAINS,
    REFERENCES,
    DEPENDS_ON,
    INDEXES,
    TRIGGERS
}

public class MetadataDependency
{
    public string SourceName { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
    public MetadataDependencyType DependencyType { get; set; }
}

public class DependencyScanVisitor : IMetadataVisitor
{
    private readonly List<MetadataDependency> _dependencies = new();

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

    public IReadOnlyCollection<MetadataDependency> GetDependencies() => _dependencies.AsReadOnly();
}
