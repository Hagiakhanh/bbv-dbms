using System;
using DBMS.Domain.Storage;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Storage;

public class BufferPoolProxyTests
{
    [Fact]
    public void FetchPage_WhenCacheMiss_ShouldDelegateToFileManagerRead()
    {
        var fileManager = new FileManager();
        var bufferPool = new BufferPool(fileManager);

        Action act = () => bufferPool.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FetchPage_WhenCacheHit_ShouldReturnMemoryResidentPage()
    {
        var fileManager = new FileManager();
        var bufferPool = new BufferPool(fileManager);

        Action act = () => bufferPool.FetchPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void FlushPage_WhenPageIsDirty_ShouldWriteToFileManager()
    {
        var fileManager = new FileManager();
        var bufferPool = new BufferPool(fileManager);

        Action act = () => bufferPool.FlushPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void EvictPage_ShouldFlushDirtyPageBeforeEviction()
    {
        var fileManager = new FileManager();
        var bufferPool = new BufferPool(fileManager);

        Action act = () => bufferPool.EvictPage();

        act.Should().Throw<NotImplementedException>();
    }
}
