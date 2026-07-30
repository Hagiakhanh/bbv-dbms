using System;

namespace DBMS.Domain.DatabaseObjects.Tables;

public interface ITableBuilder
{
    void Reset(string name);
    void AddColumn(Column column);
    void AddConstraint(Constraint constraint);
    void AddIndex(Index index);
    void AddPartition(Partition partition);
    void AddTrigger(Trigger trigger);
    Table Build();
}
