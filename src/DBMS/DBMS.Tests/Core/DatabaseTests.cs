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
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void CreateSchema_ShouldRejectDuplicateSchemaName()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void DropSchema_ShouldRemoveExistingSchema()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void DropSchema_ShouldThrow_WhenSchemaDoesNotExist()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void GetSchema_ShouldReturnExistingSchema()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void GetSchema_ShouldThrow_WhenSchemaDoesNotExist()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void Backup_ShouldCreateBackupSuccessfully()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void Restore_ShouldRestoreDatabaseSuccessfully()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }
}
