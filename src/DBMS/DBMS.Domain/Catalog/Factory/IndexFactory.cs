using System;
using DBMS.Domain.Core;
using DBMS.Domain.Catalog.Strategy;
using DBMS.Domain.Catalog.Composite;

namespace DBMS.Domain.Catalog.Factory;

public class IndexFactory : IIndexFactory
{
    public Index Create(IndexType type, IndexOptions options)
    {
        return type switch
        {
            IndexType.BTREE => new BTreeIndex { Name = options.Name ?? "IDX_BTREE_" + Guid.NewGuid().ToString("N") },
            IndexType.HASH => new HashIndex { Name = options.Name ?? "IDX_HASH_" + Guid.NewGuid().ToString("N") },
            IndexType.BITMAP => new BitmapIndex { Name = options.Name ?? "IDX_BITMAP_" + Guid.NewGuid().ToString("N") },
            _ => throw new ArgumentException("Unknown index type")
        };
    }
}



