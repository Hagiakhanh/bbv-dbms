using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Table
{
    public int TableId { get; private set; }
    public string Name { get; private set; }
    
    private readonly List<Column> _columns = new();
    public IReadOnlyCollection<Column> Columns => _columns.AsReadOnly();
    
    private readonly List<Constraint> _constraints = new();
    public IReadOnlyCollection<Constraint> Constraints => _constraints.AsReadOnly();
    
    private readonly List<Index> _indexes = new();
    public IReadOnlyCollection<Index> Indexes => _indexes.AsReadOnly();
    
    private readonly List<Partition> _partitions = new();
    public IReadOnlyCollection<Partition> Partitions => _partitions.AsReadOnly();
    
    private readonly List<Trigger> _triggers = new();
    public IReadOnlyCollection<Trigger> Triggers => _triggers.AsReadOnly();

    public Table(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Table name cannot be empty", nameof(name));
        Name = name;
    }

    public void AddColumn(Column col) { throw new NotImplementedException(); }
    public void RemoveColumn(string name) { throw new NotImplementedException(); }
    public Column GetColumn(string name) { throw new NotImplementedException(); }
    public IReadOnlyCollection<Column> GetColumns() { throw new NotImplementedException(); }
    public void AddConstraint(Constraint constraint) { throw new NotImplementedException(); }
    public void RemoveConstraint(string name) { throw new NotImplementedException(); }
    public void AddIndex(Index index) { throw new NotImplementedException(); }
    public void RemoveIndex(string name) { throw new NotImplementedException(); }
    public void AddPartition(Partition partition) { throw new NotImplementedException(); }
    public void DropPartition(string name) { throw new NotImplementedException(); }
    public void AddTrigger(Trigger trigger) { throw new NotImplementedException(); }
    public void RemoveTrigger(string name) { throw new NotImplementedException(); }
}
