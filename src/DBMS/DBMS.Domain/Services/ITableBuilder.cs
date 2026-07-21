using DBMS.Domain.Core;
using Index = DBMS.Domain.Core.Index;

namespace DBMS.Domain.Services;

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
