using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Core;

public class SchemaTests
{
    private readonly Schema _schema;

    public SchemaTests()
    {
        _schema = new Schema("TestSchema");
    }

    [Fact]
    public void AddTable_ShouldAddTableSuccessfully()
    {
        var table = new Table("Users");

        _schema.AddTable(table);

        _schema.Tables.Should().Contain(table);
    }

    [Fact]
    public void AddTable_ShouldRejectDuplicateTableName()
    {
        var table1 = new Table("Users");
        var table2 = new Table("Users");

        _schema.AddTable(table1);
        Action act = () => _schema.AddTable(table2);

        act.Should().Throw<DuplicateTableException>();
    }

    [Fact]
    public void RemoveTable_ShouldRemoveExistingTable()
    {
        var table = new Table("OldTable");
        _schema.AddTable(table);

        _schema.RemoveTable("OldTable");
        _schema.Tables.Should().NotContain(table);
    }

    [Fact]
    public void RemoveTable_ShouldThrow_WhenTableDoesNotExist()
    {
        Action act = () => _schema.RemoveTable("NonExistentTable");

        act.Should().Throw<TableNotFoundException>();
    }

    [Fact]
    public void RemoveTable_ShouldReject_WhenReferencedByForeignKey()
    {
        var table1 = new Table("Users");
        var table2 = new Table("Orders");
        _schema.AddTable(table1);
        _schema.AddTable(table2);
        var mockRecordManager = new Mock<IRecordManager>();
        var fk = new ForeignKey("FK_Users", table1, new List<Column>(), mockRecordManager.Object);
        table2.AddConstraint(fk);

        Action act = () => _schema.RemoveTable("Users");

        act.Should().Throw<ForeignKeyReferenceException>();
    }

    [Fact]
    public void GetTable_ShouldReturnTable_WhenExists()
    {
        var table = new Table("Users");
        _schema.AddTable(table);
        
        var result = _schema.GetTable("Users");
        
        result.Should().Be(table);
    }
    
    [Fact]
    public void GetTables_ShouldReturnAllTables()
    {
        var table = new Table("Users");
        _schema.AddTable(table);
        
        var result = _schema.GetTables();
        
        result.Should().Contain(table);
    }

    [Fact]
    public void AddView_ShouldRegisterView()
    {
        var view = new View("UserView");

        _schema.AddView(view);

        _schema.Views.Should().Contain(view);
    }

    [Fact]
    public void RemoveView_ShouldRemoveView()
    {
        var view = new View("OldView");
        _schema.AddView(view);

        _schema.RemoveView("OldView");
        _schema.Views.Should().NotContain(view);
    }

    [Fact]
    public void AddProcedure_ShouldRegisterProcedure()
    {
        var proc = new StoredProcedure("GetUsers");

        _schema.AddProcedure(proc);

        _schema.Procedures.Should().Contain(proc);
    }

    [Fact]
    public void RemoveProcedure_ShouldRemoveProcedure()
    {
        var proc = new StoredProcedure("OldProc");
        _schema.AddProcedure(proc);

        _schema.RemoveProcedure("OldProc");

        _schema.Procedures.Should().NotContain(proc);
    }

    [Fact]
    public void AddSequence_ShouldRegisterSequence()
    {
        var seq = new Sequence("UserSeq");

        _schema.AddSequence(seq);

        _schema.Sequences.Should().Contain(seq);
    }
}
