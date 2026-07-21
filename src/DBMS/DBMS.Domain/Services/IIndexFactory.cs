using DBMS.Domain.Core;
using Index = DBMS.Domain.Core.Index;

namespace DBMS.Domain.Services;

public interface IIndexFactory
{
    Index Create(IndexType type, IndexOptions options);
}
