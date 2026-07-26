using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Observer;

public interface IMetadataObserver
{
    void OnMetadataChanged(MetadataEvent @event);
}

public interface IMetadataEventPublisher
{
    void Subscribe(IMetadataObserver observer);
    void Unsubscribe(IMetadataObserver observer);
    void Publish(MetadataEvent @event);
}

public class MetadataEventPublisher : IMetadataEventPublisher
{
    private readonly List<IMetadataObserver> _observers = new();

    public void Subscribe(IMetadataObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IMetadataObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Publish(MetadataEvent @event)
    {
        foreach (var observer in _observers)
        {
            observer.OnMetadataChanged(@event);
        }
    }
}
