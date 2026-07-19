using System;
using System.Linq;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Storage;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Core;

public class DatabaseTests
{
    [Fact]
    public void CreateSchema_ShouldAddSchemaToDatabase()
    {
        var db = new Database(1, "TestDB", "Admin");
        
        var schema = db.CreateSchema("dbo");

        schema.Should().NotBeNull();
        schema.Name.Should().Be("dbo");
        db.Schemas.Should().ContainSingle(s => s.Name == "dbo");
    }

    [Fact]
    public void CreateSchema_ShouldRejectDuplicateSchemaName()
    {
        var db = new Database(1, "TestDB", "Admin");
        
        Action act = () => {
            db.CreateSchema("dbo");
            db.CreateSchema("dbo");
        };

        act.Should().Throw<DuplicateSchemaException>();
    }

    [Fact]
    public void DropSchema_ShouldRemoveExistingSchema()
    {
        var db = new Database(1, "TestDB", "Admin");

        Action act = () => {
            db.CreateSchema("temp");
            db.DropSchema("temp");
        };
        
        act.Should().NotThrow();
    }

    [Fact]
    public void DropSchema_ShouldThrow_WhenSchemaDoesNotExist()
    {
        var db = new Database(1, "TestDB", "Admin");

        Action act = () => db.DropSchema("nonexistent");

        act.Should().Throw<SchemaNotFoundException>();
    }

    [Fact]
    public void GetSchema_ShouldReturnExistingSchema()
    {
        var db = new Database(1, "TestDB", "Admin");
        
        var schema = db.GetSchema("dbo");
    }

    [Fact]
    public void GetSchema_ShouldThrow_WhenSchemaDoesNotExist()
    {
        var db = new Database(1, "TestDB", "Admin");

        Action act = () => db.GetSchema("nonexistent");

        act.Should().Throw<SchemaNotFoundException>();
    }

    [Fact]
    public void GetSchemas_ShouldReturnAllSchemas()
    {
        var db = new Database(1, "TestDB", "Admin");
        
        var schemas = db.GetSchemas();
        schemas.Should().NotBeNull();
    }

    [Fact]
    public void Backup_ShouldCreateBackupSuccessfully()
    {
        var db = new Database(1, "TestDB", "Admin");
        var fileManagerMock = new Mock<IFileManager>();
        var backupPath = "/path/to/backup.bak";

        db.Backup(backupPath, fileManagerMock.Object);

        fileManagerMock.Verify(f => f.WriteToFile(backupPath, It.IsAny<byte[]>()), Times.Once);
    }

    [Fact]
    public void Restore_ShouldRestoreDatabaseSuccessfully()
    {
        var db = new Database(1, "TestDB", "Admin");
        var fileManagerMock = new Mock<IFileManager>();
        var backupPath = "/path/to/backup.bak";
        
        fileManagerMock.Setup(f => f.ReadFromFile(backupPath)).Returns(new byte[] { 1, 2, 3 });

        db.Restore(backupPath, fileManagerMock.Object);

        fileManagerMock.Verify(f => f.ReadFromFile(backupPath), Times.Once);
    }
}
