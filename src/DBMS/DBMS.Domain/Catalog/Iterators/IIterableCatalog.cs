namespace DBMS.Domain.Catalog.Iterators;

public interface IIterableCatalog
{
    ICatalogIterator CreateIterator();
}
