using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Security;
using DBMS.Domain.Server;
using DBMS.Domain.Storage;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Server;

public class DatabaseCommandTests
{
    private DatabaseManager GetDatabaseManager()
    {
        DatabaseManager.ResetInstanceForTesting();
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var storageMock = new Mock<IStorageEngine>();
        var bufferPoolMock = new Mock<IBufferPool>();
        var fileManagerMock = new Mock<IFileManager>();
        var securityMock = new Mock<ISecurityManager>();

        return DatabaseManager.Initialize(
            catalogMock.Object,
            connPoolMock.Object,
            storageMock.Object,
            bufferPoolMock.Object,
            fileManagerMock.Object,
            securityMock.Object);
    }

    [Fact]
    public void CreateDatabaseCommand_Execute_ShouldThrowNotImplementedException()
    {
        var manager = GetDatabaseManager();
        var cmd = new CreateDatabaseCommand(manager, "TestDB");

        Action act = () => cmd.Execute();
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DropDatabaseCommand_Execute_ShouldThrowNotImplementedException()
    {
        var manager = GetDatabaseManager();
        var cmd = new DropDatabaseCommand(manager, "TestDB", cascade: true);

        Action act = () => cmd.Execute();
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RenameDatabaseCommand_Execute_ShouldThrowNotImplementedException()
    {
        var manager = GetDatabaseManager();
        var cmd = new RenameDatabaseCommand(manager, "OldDB", "NewDB");

        Action act = () => cmd.Execute();
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DatabaseCommandExecutor_Execute_ShouldThrowNotImplementedException()
    {
        var executor = new DatabaseCommandExecutor();
        var cmdMock = new Mock<IDatabaseCommand>();

        Action act = () => executor.Execute(cmdMock.Object);
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DatabaseCommandExecutor_GetHistory_ShouldThrowNotImplementedException()
    {
        var executor = new DatabaseCommandExecutor();

        Action act = () => executor.GetHistory();
        act.Should().Throw<NotImplementedException>();
    }
}
