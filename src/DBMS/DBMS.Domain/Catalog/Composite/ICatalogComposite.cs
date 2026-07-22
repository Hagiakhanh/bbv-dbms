using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Composite;

public interface ICatalogComposite : ICatalogComponent
{
    IReadOnlyCollection<ICatalogComponent> Children { get; }
}
