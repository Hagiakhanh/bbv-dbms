namespace DBMS.Domain.Catalog.Iterator;

public interface IIterableCatalog
{
    ICatalogIterator CreateIterator();
}
