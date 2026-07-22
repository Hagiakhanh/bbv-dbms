using System;
using System.Collections.Generic;
using System.Linq;

namespace DBMS.Domain.Catalog.Composite;

public class Table : ICatalogComposite, IIterableCatalog
{
    public int TableId { get; private set; }
    public string Name { get; private set; }
    public Schema Parent { get; set; }
    
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

    public IReadOnlyCollection<ICatalogComponent> Children 
    {
        get
        {
            var children = new List<ICatalogComponent>();
            children.AddRange(_columns);
            children.AddRange(_constraints);
            children.AddRange(_indexes);
            children.AddRange(_partitions);
            children.AddRange(_triggers);
            return children.AsReadOnly();
        }
    }

    public Iterator.ICatalogIterator CreateIterator()
    {
        throw new System.NotImplementedException();
    }

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
    public void RemovePartition(string name) { throw new NotImplementedException(); }
    public void AddTrigger(Trigger trigger) { throw new NotImplementedException(); }
    public void RemoveTrigger(string name) { throw new NotImplementedException(); }
    public virtual Row LookupReferencedRow(Row row) { throw new NotImplementedException(); }
}

