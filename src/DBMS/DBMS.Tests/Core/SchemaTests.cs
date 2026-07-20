using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
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
    public void DropTable_ShouldRemoveExistingTable()
    {
        var table = new Table("OldTable");
        _schema.AddTable(table);

        _schema.DropTable("OldTable");
        _schema.Tables.Should().NotContain(table);
    }

    [Fact]
    public void DropTable_ShouldThrow_WhenTableDoesNotExist()
    {
        Action act = () => _schema.DropTable("NonExistentTable");

        act.Should().Throw<TableNotFoundException>();
    }

    [Fact]
    public void DropTable_ShouldReject_WhenReferencedByForeignKey()
    {
        var table1 = new Table("Users");
        var table2 = new Table("Orders");
        _schema.AddTable(table1);
        _schema.AddTable(table2);

        var fk = new ForeignKey { ReferenceTable = table1 };
        table2.AddConstraint(fk);

        Action act = () => _schema.DropTable("Users");

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
    public void CreateView_ShouldRegisterView()
    {
        var view = new View("UserView");

        _schema.CreateView(view);

        _schema.Views.Should().Contain(view);
    }

    [Fact]
    public void DropView_ShouldRemoveView()
    {
        var view = new View("OldView");
        _schema.CreateView(view);

        _schema.DropView("OldView");
        _schema.Views.Should().NotContain(view);
    }

    [Fact]
    public void CreateProcedure_ShouldRegisterProcedure()
    {
        var proc = new StoredProcedure("GetUsers");

        _schema.CreateProcedure(proc);

        _schema.Procedures.Should().Contain(proc);
    }

    [Fact]
    public void DropProcedure_ShouldRemoveProcedure()
    {
        var proc = new StoredProcedure("OldProc");
        _schema.CreateProcedure(proc);

        _schema.DropProcedure("OldProc");

        _schema.Procedures.Should().NotContain(proc);
    }

    [Fact]
    public void CreateSequence_ShouldRegisterSequence()
    {
        var seq = new Sequence("UserSeq");

        _schema.CreateSequence(seq);

        _schema.Sequences.Should().Contain(seq);
    }
}
