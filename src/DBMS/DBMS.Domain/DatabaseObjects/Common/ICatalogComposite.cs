using System.Collections.Generic;

namespace DBMS.Domain.DatabaseObjects.Common;

public interface ICatalogComposite : ICatalogComponent
{
    IReadOnlyCollection<ICatalogComponent> Children { get; }
}
