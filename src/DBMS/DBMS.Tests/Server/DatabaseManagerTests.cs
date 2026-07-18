using System;
using System.Collections.Generic;
using DBMS.Domain.Server;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Server;

public class DatabaseManagerTests
{
    [Fact]
    public void CreateDatabase_ShouldCreateDatabaseSuccessfully()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void CreateDatabase_ShouldRejectDuplicateDatabaseName()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropDatabase_ShouldRemoveDatabaseSuccessfully()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropDatabase_ShouldRejectOpenDatabase()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void OpenDatabase_ShouldLoadStorageAndCatalog()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void CloseDatabase_ShouldFlushDirtyBuffers()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void GetDatabase_ShouldReturnExistingDatabase()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void ListDatabases_ShouldReturnAllDatabases()
    {
        // Arrange
        
        // Act

        // Assert
    }
}
