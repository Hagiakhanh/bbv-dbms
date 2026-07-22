using DBMS.Domain.Core;

namespace DBMS.Domain.Catalog.Builder;

public class TableBuilder : ITableBuilder
{
    private Table _currentTable;

    public void Reset(string name)
    {
        _currentTable = new Table(name);
    }

    public void AddColumn(Column column)
    {
        _currentTable.AddColumn(column);
    }

    public void AddConstraint(Constraint constraint)
    {
        _currentTable.AddConstraint(constraint);
    }

    public void AddIndex(Index index)
    {
        _currentTable.AddIndex(index);
    }

    public void AddPartition(Partition partition)
    {
        _currentTable.AddPartition(partition);
    }

    public void AddTrigger(Trigger trigger)
    {
        _currentTable.AddTrigger(trigger);
    }

    public Table Build()
    {
        var table = _currentTable;
        _currentTable = null;
        return table;
    }
}



