using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Core;

public class DatabaseTests
{
    [Fact]
    public void CreateSchema_ShouldAddSchemaToDatabase()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void CreateSchema_ShouldRejectDuplicateSchemaName()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropSchema_ShouldRemoveExistingSchema()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropSchema_ShouldThrow_WhenSchemaDoesNotExist()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void GetSchema_ShouldReturnExistingSchema()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void GetSchema_ShouldThrow_WhenSchemaDoesNotExist()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void Backup_ShouldCreateBackupSuccessfully()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void Restore_ShouldRestoreDatabaseSuccessfully()
    {
        // Arrange
        
        // Act

        // Assert
    }
}
