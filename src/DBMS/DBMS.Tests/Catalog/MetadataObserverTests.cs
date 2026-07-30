using System;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Catalog;

public class MetadataObserverTests
{
    [Fact]
    public void MetadataEventPublisher_Publish_ShouldNotifySubscribedObservers()
    {
        var publisher = new MetadataEventPublisher();
        var observerMock = new Mock<IMetadataObserver>();

        publisher.Subscribe(observerMock.Object);

        var evt = new MetadataEvent
        {
            EventType = MetadataEventType.CREATED,
            ObjectName = "UsersTable"
        };

        publisher.Publish(evt);

        observerMock.Verify(o => o.OnMetadataChanged(evt), Times.Once);
    }

    [Fact]
    public void MetadataEventPublisher_Unsubscribe_ShouldStopNotifyingObserver()
    {
        var publisher = new MetadataEventPublisher();
        var observerMock = new Mock<IMetadataObserver>();

        publisher.Subscribe(observerMock.Object);
        publisher.Unsubscribe(observerMock.Object);

        var evt = new MetadataEvent
        {
            EventType = MetadataEventType.REMOVED,
            ObjectName = "UsersTable"
        };

        publisher.Publish(evt);

        observerMock.Verify(o => o.OnMetadataChanged(It.IsAny<MetadataEvent>()), Times.Never);
    }

    [Fact]
    public void CatalogCacheObserver_OnMetadataChanged_ShouldThrowNotImplementedException()
    {
        var observer = new CatalogCacheObserver();
        var evt = new MetadataEvent();

        Action act = () => observer.OnMetadataChanged(evt);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void MetadataStatisticsObserver_OnMetadataChanged_ShouldThrowNotImplementedException()
    {
        var observer = new MetadataStatisticsObserver();
        var evt = new MetadataEvent();

        Action act = () => observer.OnMetadataChanged(evt);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void MetadataAuditObserver_OnMetadataChanged_ShouldThrowNotImplementedException()
    {
        var observer = new MetadataAuditObserver();
        var evt = new MetadataEvent();

        Action act = () => observer.OnMetadataChanged(evt);

        act.Should().Throw<NotImplementedException>();
    }
}
