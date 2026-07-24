using System;
using DBMS.Domain.Storage;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Storage;

public class BufferPoolTests
{
    [Fact]
    public void FetchPage_ShouldLoadPageIntoBuffer()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FetchPage_ShouldReturnCachedPage_WhenAlreadyLoaded()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FlushPage_ShouldWriteDirtyPageToDisk()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.FlushPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FlushDirtyPage_ShouldWriteWALBeforeDisk()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.FlushPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void EvictPage_ShouldUseReplacementPolicy()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.EvictPage();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void EvictPage_ShouldNotEvictPinnedPage()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.EvictPage();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FetchPage_ShouldPinPageWhileInUse()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void UnpinPage_ShouldDecreasePinCount()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.UnpinPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FlushAll_ShouldSyncAllDirtyPagesToDisk()
    {
        var bufferPool = new BufferPool();

        Action act = () => bufferPool.FlushAll();

        act.Should().Throw<NotImplementedException>();
    }
}
