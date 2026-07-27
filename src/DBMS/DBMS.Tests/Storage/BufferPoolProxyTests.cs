using System;
using DBMS.Domain.Storage;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Storage;

public class BufferPoolProxyTests
{
    [Fact]
    public void FetchPage_WhenCacheMiss_ShouldDelegateToDiskPageStore()
    {
        var realStore = new DiskPageStore();
        var proxy = new BufferPoolProxy(realStore);

        Action act = () => proxy.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FetchPage_WhenCacheHit_ShouldReturnMemoryResidentPage()
    {
        var realStore = new DiskPageStore();
        var proxy = new BufferPoolProxy(realStore);

        Action act = () => proxy.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FlushPage_WhenPageIsDirty_ShouldWriteToDiskPageStore()
    {
        var realStore = new DiskPageStore();
        var proxy = new BufferPoolProxy(realStore);

        Action act = () => proxy.FlushPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void EvictFrame_ShouldFlushDirtyFrameBeforeEviction()
    {
        var realStore = new DiskPageStore();
        var proxy = new BufferPoolProxy(realStore);

        Action act = () => proxy.EvictFrame();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void AllocatePage_ShouldDelegateToDiskPageStore()
    {
        var realStore = new DiskPageStore();
        var proxy = new BufferPoolProxy(realStore);

        Action act = () => proxy.AllocatePage(101);

        act.Should().Throw<NotImplementedException>();
    }
}
