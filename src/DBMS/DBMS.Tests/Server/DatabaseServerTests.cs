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
    private (DatabaseServer server, Mock<IDbEngineFacade> facadeMock) CreateServer()
    {
        var facadeMock = new Mock<IDbEngineFacade>();
        var server = new DatabaseServer(facadeMock.Object);

        return (server, facadeMock);
    }

    [Fact]
    public void Start_ShouldInitializeAllServices()
    {
        var (server, facadeMock) = CreateServer();

        server.Start(safeMode: true);

        facadeMock.Verify(f => f.Start(true), Times.Once);
        server.Status.Should().Be(ServerStatus.Running);
    }

    [Fact]
    public void Stop_ShouldStopEngine()
    {
        var (server, facadeMock) = CreateServer();

        server.Stop(force: false);

        facadeMock.Verify(f => f.Stop(false), Times.Once);
        server.Status.Should().Be(ServerStatus.Stopped);
    }

    [Fact]
    public void RecoverAfterCrash_ShouldCallFacadeRecover()
    {
        var (server, facadeMock) = CreateServer();

        server.Recover();

        facadeMock.Verify(f => f.Recover(), Times.Once);
    }
}
