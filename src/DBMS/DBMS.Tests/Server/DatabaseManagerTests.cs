using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Catalog;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Server;
using DBMS.Domain.Storage;
using DBMS.Domain.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Server;

public class DatabaseManagerTests
{
    private (DatabaseManager manager, Mock<ICatalogManager> catalog, Mock<IConnectionPool> connPool, Mock<IStorageEngine> storage, Mock<IBufferPool> bufferPool, Mock<IFileManager> fileManager, Mock<ISecurityManager> securityManager) CreateManager()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var storageMock = new Mock<IStorageEngine>();
        var bufferPoolMock = new Mock<IBufferPool>();
        var fileManagerMock = new Mock<IFileManager>();
        var securityManagerMock = new Mock<ISecurityManager>();
        
        var manager = new DatabaseManager(
            catalogMock.Object, 
            connPoolMock.Object, 
            storageMock.Object, 
            bufferPoolMock.Object, 
            fileManagerMock.Object,
            securityManagerMock.Object);
            
        return (manager, catalogMock, connPoolMock, storageMock, bufferPoolMock, fileManagerMock, securityManagerMock);
    }

    [Fact]
    public void CreateDatabase_ShouldCreateDatabaseSuccessfully()
    {
        var (manager, catalog, _, _, _, _, security) = CreateManager();
        var dbName = "TestDB";
        
        catalog.Setup(c => c.CheckExists(dbName)).Returns(false);

        manager.CreateDatabase(dbName);

        catalog.Verify(c => c.RegisterDatabase(dbName), Times.Once);
    }

    [Fact]
    public void CreateDatabase_ShouldRejectDuplicateDatabaseName()
    {
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        var dbName = "TestDB";

        catalog.Setup(c => c.CheckExists(dbName)).Returns(true);
        
        Action act = () => manager.CreateDatabase(dbName);

        act.Should().Throw<DuplicateNameException>();
        catalog.Verify(c => c.RegisterDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void CreateDatabase_ShouldRejectInvalidName()
    {
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        var invalidDbName = "";
        
        Action act = () => manager.CreateDatabase(invalidDbName);

        act.Should().Throw<InvalidNameException>();
        catalog.Verify(c => c.CheckExists(It.IsAny<string>()), Times.Never);
        catalog.Verify(c => c.RegisterDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void CreateDatabase_ShouldReject_WhenPermissionDenied()
    {
        var (manager, catalog, _, _, _, _, security) = CreateManager();
        var dbName = "TestDB";

        security.Setup(s => s.CheckPermission(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(false);
        
        Action act = () => manager.CreateDatabase(dbName);

        act.Should().Throw<PermissionDeniedException>();
        catalog.Verify(c => c.RegisterDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void DropDatabase_ShouldRemoveDatabaseSuccessfully()
    {
        var (manager, catalog, connPool, _, _, _, _) = CreateManager();
        var dbName = "TestDB";
        
        catalog.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPool.Setup(cp => cp.HasActiveConnections(dbName)).Returns(false);
        
        manager.DropDatabase(dbName, cascade: false);

        catalog.Verify(c => c.RemoveDatabase(dbName), Times.Once);
    }

    [Fact]
    public void DropDatabase_ShouldRejectOpenDatabase()
    {
        var (manager, catalog, connPool, _, _, _, _) = CreateManager();
        var dbName = "TestDB";
        
        catalog.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPool.Setup(cp => cp.HasActiveConnections(dbName)).Returns(true);
        
        Action act = () => manager.DropDatabase(dbName, cascade: false);

        act.Should().Throw<DatabaseInUseException>();
        catalog.Verify(c => c.RemoveDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void DropDatabase_ShouldForceCloseConnections_WhenCascade()
    {
        var (manager, catalog, connPool, _, _, _, _) = CreateManager();
        var dbName = "TestDB";
        
        catalog.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPool.Setup(cp => cp.HasActiveConnections(dbName)).Returns(true);

        manager.DropDatabase(dbName, cascade: true);

        connPool.Verify(cp => cp.ForceCloseConnections(dbName), Times.Once);
        catalog.Verify(c => c.RemoveDatabase(dbName), Times.Once);
    }

    [Fact]
    public void DropDatabase_ShouldReject_WhenDatabaseContainsSchemas()
    {
        var (manager, catalog, connPool, _, _, _, _) = CreateManager();
        var dbName = "TestDB";
        
        catalog.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPool.Setup(cp => cp.HasActiveConnections(dbName)).Returns(false);
        catalog.Setup(c => c.HasSchemas(dbName)).Returns(true); // Mock HasSchemas logic
        
        Action act = () => manager.DropDatabase(dbName, cascade: false);

        act.Should().Throw<DatabaseContainsSchemasException>();
        catalog.Verify(c => c.RemoveDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void OpenDatabase_ShouldLoadStorageAndCatalog()
    { 
        var (manager, catalog, _, storage, _, _, _) = CreateManager();
        var dbName = "MyDb";

        catalog.Setup(c => c.GetDatabaseState(dbName)).Returns(DatabaseState.Online);

        manager.OpenDatabase(dbName);

        storage.Verify(s => s.InitializeStorageEngine(dbName), Times.Once);
        catalog.Verify(c => c.LoadCatalog(dbName), Times.Once);
    }

    [Fact]
    public void OpenDatabase_ShouldReject_WhenDatabaseIsOffline()
    { 
        var (manager, catalog, _, storage, _, _, _) = CreateManager();
        var dbName = "MyDb";

        catalog.Setup(c => c.GetDatabaseState(dbName)).Returns(DatabaseState.Offline);

        Action act = () => manager.OpenDatabase(dbName);

        act.Should().Throw<DatabaseOfflineException>();
        storage.Verify(s => s.InitializeStorageEngine(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void CloseDatabase_ShouldFlushDirtyBuffers()
    { 
        var (manager, _, _, _, bufferPool, _, _) = CreateManager();
        var dbName = "MyDb";

        manager.CloseDatabase(dbName);

        bufferPool.Verify(b => b.FlushDirtyBuffers(dbName), Times.Once);
    }

    [Fact]
    public void GetDatabase_ShouldReturnExistingDatabase()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        var dbName = "MyDb";
        var db = new Database();
        catalog.Setup(c => c.GetDatabase(dbName)).Returns(db);

        var result = manager.GetDatabase(dbName);

        result.Should().Be(db);
    }

    [Fact]
    public void ListDatabases_ShouldReturnAllDatabases()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        var databases = new List<Database> { new Database(), new Database() };
        catalog.Setup(c => c.ListDatabases()).Returns(databases);

        var result = manager.ListDatabases();

        result.Should().BeEquivalentTo(databases);
    }

    [Fact]
    public void RenameDatabase_ShouldUpdateNameSuccessfully()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        
        catalog.Setup(c => c.CheckExists("NewName")).Returns(false);

        manager.RenameDatabase("MyDb", "NewName");

        catalog.Verify(c => c.UpdateDatabaseName("MyDb", "NewName"), Times.Once);
    }

    [Fact]
    public void RenameDatabase_ShouldRejectDuplicateName()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        
        catalog.Setup(c => c.CheckExists("NewName")).Returns(true);

        Action act = () => manager.RenameDatabase("MyDb", "NewName");

        act.Should().Throw<DuplicateNameException>();
        catalog.Verify(c => c.UpdateDatabaseName(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void SetDatabaseState_ShouldSetToReadOnly()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();

        manager.SetDatabaseState("MyDb", DatabaseState.ReadOnly);

        catalog.Verify(c => c.UpdateState("MyDb", DatabaseState.ReadOnly), Times.Once);
    }

    [Fact]
    public void SetDatabaseState_ShouldSetToOffline()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();

        manager.SetDatabaseState("MyDb", DatabaseState.Offline);

        catalog.Verify(c => c.UpdateState("MyDb", DatabaseState.Offline), Times.Once);
    }

    [Fact]
    public void AttachDatabase_ShouldRegisterExistingDatabaseFiles()
    { 
        var (manager, catalog, _, _, _, fileManager, _) = CreateManager();
        var dbName = "ArchivedDb";
        var path = "/data/archived.db";

        fileManager.Setup(f => f.ValidateFilesExist(path)).Returns(true);

        manager.AttachDatabase(dbName, path);

        fileManager.Verify(f => f.ValidateFilesExist(path), Times.Once);
        catalog.Verify(c => c.RegisterExistingDatabaseFiles(dbName, path), Times.Once);
    }

    [Fact]
    public void DetachDatabase_ShouldUnregisterButKeepFiles()
    { 
        var (manager, catalog, _, _, _, _, _) = CreateManager();
        var dbName = "ArchivedDb";

        manager.DetachDatabase(dbName);

        catalog.Verify(c => c.Unregister(dbName), Times.Once);
    }
}

