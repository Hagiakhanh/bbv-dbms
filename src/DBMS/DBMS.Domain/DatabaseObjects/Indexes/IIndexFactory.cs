using DBMS.Domain.Core;

namespace DBMS.Domain.DatabaseObjects.Indexes;

public interface IIndexFactory
{
    Index Create(IndexType type, IndexOptions options);
}



