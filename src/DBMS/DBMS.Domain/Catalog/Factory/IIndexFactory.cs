using DBMS.Domain.Core;

namespace DBMS.Domain.Catalog.Factory;

public interface IIndexFactory
{
    Index Create(IndexType type, IndexOptions options);
}



