using System;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Server;
using DBMS.Domain.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Server;

public class DatabaseServerTests
{
    private (DatabaseServer server, Mock<IDbEngineFacade> facadeMock, Mock<IDatabaseManager> dbManagerMock, Mock<IConfigurationManager> configMock, Mock<ISecurityManager> securityMock, Mock<IMonitoringManager> monitoringMock) CreateServer()
    {
        var facadeMock = new Mock<IDbEngineFacade>();
        var dbManagerMock = new Mock<IDatabaseManager>();
        var configMock = new Mock<IConfigurationManager>();
        var securityMock = new Mock<ISecurityManager>();
        var monitoringMock = new Mock<IMonitoringManager>();

        var server = new DatabaseServer(
            facadeMock.Object,
            dbManagerMock.Object,
            configMock.Object,
            securityMock.Object,
            monitoringMock.Object);

        return (server, facadeMock, dbManagerMock, configMock, securityMock, monitoringMock);
    }

    [Fact]
    public void Start_FromStoppedState_ShouldTransitionToRunning()
    {
        var (server, facadeMock, _, configMock, _, monitoringMock) = CreateServer();

        server.Status.Should().Be(ServerStatus.Stopped);

        server.Start(safeMode: true);

        // State changes to Running
        server.Status.Should().Be(ServerStatus.Running);

        // Components are initialized
        configMock.Verify(c => c.LoadConfiguration("default.config"), Times.Once);
        facadeMock.Verify(f => f.Start(true), Times.Once);
        monitoringMock.Verify(m => m.StartCollection(), Times.Once);
    }

    [Fact]
    public void Start_WhenAlreadyRunning_ShouldThrowException()
    {
        var (server, _, _, _, _, _) = CreateServer();

        server.Start(safeMode: true);
        server.Status.Should().Be(ServerStatus.Running);

        Action act = () => server.Start(safeMode: true);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Server is already running.");
    }

    [Fact]
    public void Stop_FromRunningState_ShouldTransitionToStopped()
    {
        var (server, facadeMock, _, _, _, _) = CreateServer();

        server.Start(safeMode: false);
        server.Stop(force: false);

        server.Status.Should().Be(ServerStatus.Stopped);
        facadeMock.Verify(f => f.Stop(false), Times.Once);
    }

    [Fact]
    public void Stop_FromStoppedState_ShouldDoNothing()
    {
        var (server, facadeMock, _, _, _, _) = CreateServer();

        server.Status.Should().Be(ServerStatus.Stopped);
        server.Stop(force: false);

        server.Status.Should().Be(ServerStatus.Stopped);
        facadeMock.Verify(f => f.Stop(It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public void Recover_FromStoppedState_ShouldTransitionThroughRecovering()
    {
        var (server, facadeMock, _, _, _, _) = CreateServer();

        server.Status.Should().Be(ServerStatus.Stopped);
        server.Recover();

        // After recover completes, it goes back to Stopped
        server.Status.Should().Be(ServerStatus.Stopped);
        facadeMock.Verify(f => f.Recover(), Times.Once);
    }

    [Fact]
    public void Recover_WhenRunning_ShouldThrowException()
    {
        var (server, _, _, _, _, _) = CreateServer();

        server.Start(safeMode: false);
        
        Action act = () => server.Recover();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot recover while running.");
    }
}
