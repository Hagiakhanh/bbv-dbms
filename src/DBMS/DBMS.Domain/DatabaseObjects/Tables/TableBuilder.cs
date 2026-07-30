using System;

namespace DBMS.Domain.DatabaseObjects.Tables;

public class TableBuilder : ITableBuilder
{
    private Table _currentTable;

    public void Reset(string name)
    {
        throw new NotImplementedException();
    }

    public void AddColumn(Column column)
    {
        throw new NotImplementedException();
    }

    public void AddConstraint(Constraint constraint)
    {
        throw new NotImplementedException();
    }

    public void AddIndex(Index index)
    {
        throw new NotImplementedException();
    }

    public void AddPartition(Partition partition)
    {
        throw new NotImplementedException();
    }

    public void AddTrigger(Trigger trigger)
    {
        throw new NotImplementedException();
    }

    public Table Build()
    {
        throw new NotImplementedException();
    }
}
