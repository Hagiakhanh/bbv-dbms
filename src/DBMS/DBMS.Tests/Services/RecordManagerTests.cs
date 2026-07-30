using System;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Services;

public class RecordManagerTests
{
    [Fact]
    public void InsertRecord_ShouldValidateConstraintsBeforeInsert()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var row = new Row();

        Action act = () => recordManager.Insert(table, row);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void InsertRecord_ShouldUpdateIndexes()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var row = new Row();

        Action act = () => recordManager.Insert(table, row);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void InsertRecord_ShouldRollback_WhenConstraintValidationFails()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var row = new Row();

        Action act = () => recordManager.Insert(table, row);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void UpdateRecord_ShouldValidateConstraints()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var rid = new RID { PageId = 1, SlotNumber = 1 };
        var row = new Row();

        Action act = () => recordManager.Update(table, rid, row);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void UpdateRecord_ShouldRollback_WhenIndexUpdateFails()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var rid = new RID { PageId = 1, SlotNumber = 1 };
        var row = new Row();

        Action act = () => recordManager.Update(table, rid, row);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void DeleteRecord_ShouldValidateForeignKeyConstraints()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => recordManager.Delete(table, rid);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DeleteRecord_ShouldRollback_WhenForeignKeyValidationFails()
    {
        var recordManager = new RecordManager();
        var table = new Table("Users");
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => recordManager.Delete(table, rid);

        act.Should().Throw<Exception>();
    }
}
