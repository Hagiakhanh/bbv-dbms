using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Catalog;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Server;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Server;

public class DatabaseManagerTests
{
    [Fact]
    public void CreateDatabase_ShouldCreateDatabaseSuccessfully()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var manager = new DatabaseManager(catalogMock.Object, connPoolMock.Object);
        var dbName = "TestDB";
        
        catalogMock.Setup(c => c.CheckExists(dbName)).Returns(false);

        manager.CreateDatabase(dbName);

        catalogMock.Verify(c => c.RegisterDatabase(dbName), Times.Once);
    }

    [Fact]
    public void CreateDatabase_ShouldRejectDuplicateDatabaseName()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var manager = new DatabaseManager(catalogMock.Object, connPoolMock.Object);
        var dbName = "TestDB";

        catalogMock.Setup(c => c.CheckExists(dbName)).Returns(true);
        
        Action act = () => manager.CreateDatabase(dbName);

        act.Should().Throw<DuplicateNameException>();
        catalogMock.Verify(c => c.RegisterDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void CreateDatabase_ShouldRejectInvalidName()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var manager = new DatabaseManager(catalogMock.Object, connPoolMock.Object);
        var invalidDbName = "";
        
        Action act = () => manager.CreateDatabase(invalidDbName);

        act.Should().Throw<InvalidNameException>();
        catalogMock.Verify(c => c.CheckExists(It.IsAny<string>()), Times.Never);
        catalogMock.Verify(c => c.RegisterDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void DropDatabase_ShouldRemoveDatabaseSuccessfully()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var manager = new DatabaseManager(catalogMock.Object, connPoolMock.Object);
        var dbName = "TestDB";
        
        catalogMock.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPoolMock.Setup(cp => cp.HasActiveConnections(dbName)).Returns(false);
        
        manager.DropDatabase(dbName, cascade: false);

        catalogMock.Verify(c => c.RemoveDatabase(dbName), Times.Once);
    }

    [Fact]
    public void DropDatabase_ShouldRejectOpenDatabase()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var manager = new DatabaseManager(catalogMock.Object, connPoolMock.Object);
        var dbName = "TestDB";
        
        catalogMock.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPoolMock.Setup(cp => cp.HasActiveConnections(dbName)).Returns(true); // Active connections > 0
        
        Action act = () => manager.DropDatabase(dbName, cascade: false);

        act.Should().Throw<DatabaseInUseException>();
        catalogMock.Verify(c => c.RemoveDatabase(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void DropDatabase_ShouldForceCloseConnections_WhenCascade()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var connPoolMock = new Mock<IConnectionPool>();
        var manager = new DatabaseManager(catalogMock.Object, connPoolMock.Object);
        var dbName = "TestDB";
        
        catalogMock.Setup(c => c.CheckExists(dbName)).Returns(true);
        connPoolMock.Setup(cp => cp.HasActiveConnections(dbName)).Returns(true);

        manager.DropDatabase(dbName, cascade: true);

        connPoolMock.Verify(cp => cp.ForceCloseConnections(dbName), Times.Once);
        catalogMock.Verify(c => c.RemoveDatabase(dbName), Times.Once);
    }

    [Fact]
    public void OpenDatabase_ShouldLoadStorageAndCatalog()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void OpenDatabase_ShouldReject_WhenDatabaseIsOffline()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void CloseDatabase_ShouldFlushDirtyBuffers()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void GetDatabase_ShouldReturnExistingDatabase()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void ListDatabases_ShouldReturnAllDatabases()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void RenameDatabase_ShouldUpdateNameSuccessfully()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void RenameDatabase_ShouldRejectDuplicateName()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void SetDatabaseState_ShouldSetToReadOnly()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void SetDatabaseState_ShouldSetToOffline()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void AttachDatabase_ShouldRegisterExistingDatabaseFiles()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }

    [Fact]
    public void DetachDatabase_ShouldUnregisterButKeepFiles()
    { 
        throw new NotImplementedException("Test not implemented yet."); 
    }
}

