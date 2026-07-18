using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Core;

public class SchemaTests
{
    [Fact]
    public void AddTable_ShouldAddTableSuccessfully()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void AddTable_ShouldRejectDuplicateTableName()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropTable_ShouldRemoveExistingTable()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropTable_ShouldThrow_WhenTableDoesNotExist()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void CreateView_ShouldRegisterView()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropView_ShouldRemoveView()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void CreateProcedure_ShouldRegisterProcedure()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void DropProcedure_ShouldRemoveProcedure()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void CreateSequence_ShouldRegisterSequence()
    {
        // Arrange
        
        // Act

        // Assert
    }
}
