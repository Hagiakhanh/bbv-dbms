using System;

namespace DBMS.Domain.Catalog.Iterators;

public class CatalogIterator : ICatalogIterator
{
    private readonly ICatalogComposite _collection;
    private int _iterationState;

    public CatalogIterator(ICatalogComposite collection)
    {
        _collection = collection;
        _iterationState = 0;
    }

    public ICatalogComponent GetNext()
    {
        throw new NotImplementedException();
    }

    public bool HasMore()
    {
        throw new NotImplementedException();
    }
}
