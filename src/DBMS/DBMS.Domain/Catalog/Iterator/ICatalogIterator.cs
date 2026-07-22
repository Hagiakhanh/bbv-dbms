using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Iterator;

public interface ICatalogIterator
{
    ICatalogComponent GetNext();
    bool HasMore();
}
