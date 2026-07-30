
namespace DBMS.Domain.Catalog.Iterators;

public interface ICatalogIterator
{
    ICatalogComponent GetNext();
    bool HasMore();
}
