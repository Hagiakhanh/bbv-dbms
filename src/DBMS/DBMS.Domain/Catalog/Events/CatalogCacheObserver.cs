using System;

namespace DBMS.Domain.Catalog.Events;

public class CatalogCacheObserver : IMetadataObserver
{
    public void OnMetadataChanged(MetadataEvent @event)
    {
        throw new NotImplementedException();
    }
}

public class MetadataStatisticsObserver : IMetadataObserver
{
    public void OnMetadataChanged(MetadataEvent @event)
    {
        throw new NotImplementedException();
    }
}

public class MetadataAuditObserver : IMetadataObserver
{
    public void OnMetadataChanged(MetadataEvent @event)
    {
        throw new NotImplementedException();
    }
}
