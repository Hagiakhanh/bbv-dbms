using DBMS.Domain.Core;
using DBMS.Domain.Catalog.Strategy;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Factory;

public interface IIndexFactory
{
    Index Create(IndexType type, IndexOptions options);
}



