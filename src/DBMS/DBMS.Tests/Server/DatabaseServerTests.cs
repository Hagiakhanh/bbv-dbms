using System;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Server;
using DBMS.Domain.Storage;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Server;

public class DatabaseServerTests
{
    private (DatabaseServer server, Mock<IBufferPool> bufferPool, Mock<IWALManager> walManager) CreateServer()
    {
        var bufferPoolMock = new Mock<IBufferPool>();
        var walManagerMock = new Mock<IWALManager>();

        var server = new DatabaseServer(bufferPoolMock.Object, walManagerMock.Object);

        return (server, bufferPoolMock, walManagerMock);
    }

    [Fact]
    public void Start_ShouldInitializeAllServices()
    {
        var (server, _, _) = CreateServer();

        server.Start(safeMode: true);

        // We expect the server status to be updated to Running after starting.
        server.Status.Should().Be(ServerStatus.Running);
    }

    [Fact]
    public void Stop_ShouldFlushDirtyPagesBeforeShutdown()
    {
        var (server, bufferPool, _) = CreateServer();

        server.Stop(force: false);

        bufferPool.Verify(b => b.FlushDirtyPagesBeforeShutdown(), Times.Once);
        server.Status.Should().Be(ServerStatus.Stopped);
    }

    [Fact]
    public void RecoverAfterCrash_ShouldReplayWAL()
    {
        var (server, _, walManager) = CreateServer();

        server.Recover();

        walManager.Verify(w => w.ReplayWAL(), Times.Once);
    }
}
