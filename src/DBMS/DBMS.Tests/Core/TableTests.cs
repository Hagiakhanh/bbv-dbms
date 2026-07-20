using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Core;

public class TableTests
{
    private readonly Table _table;

    public TableTests()
    {
        _table = new Table("Users");
    }

    [Fact]
    public void AddColumn_ShouldAddColumnSuccessfully()
    {
        var col = new Column { Name = "Id", DataType = DataTypeEnum.INT };
        
        _table.AddColumn(col);

        _table.Columns.Should().Contain(col);
    }

    [Fact]
    public void AddColumn_ShouldRejectDuplicateColumnName()
    {
        var col1 = new Column { Name = "Id" };
        var col2 = new Column { Name = "Id" };
        
        _table.AddColumn(col1);

        Action act = () => _table.AddColumn(col2);

        act.Should().Throw<DuplicateColumnException>();
    }

    [Fact]
    public void RemoveColumn_ShouldRemoveExistingColumn()
    {
        var col = new Column { Name = "Age" };
        _table.AddColumn(col);

        _table.RemoveColumn("Age");

        _table.Columns.Should().NotContain(col);
    }

    [Fact]
    public void DropColumn_ShouldReject_WhenReferencedByConstraint()
    {
        var col = new Column { Name = "Id" };
        _table.AddColumn(col);

        var pk = new PrimaryKey { Columns = new List<Column> { col } };
        _table.AddConstraint(pk);

        Action act = () => _table.RemoveColumn("Id");

        act.Should().Throw<ColumnReferencedByConstraintException>();
    }

    [Fact]
    public void AddConstraint_ShouldRegisterConstraint()
    {
        var constraint = new PrimaryKey { Columns = new List<Column> { new Column { Name = "Id" } } };

        _table.AddConstraint(constraint);

        _table.Constraints.Should().Contain(constraint);
    }

    [Fact]
    public void RemoveConstraint_ShouldRemoveConstraint()
    {
        var constraint = new PrimaryKey { Columns = new List<Column>() };
        _table.AddConstraint(constraint);

        _table.RemoveConstraint(constraint.Name);

        _table.Constraints.Should().NotContain(constraint);
    }

    [Fact]
    public void AddIndex_ShouldRegisterIndex()
    {
        var index = new BTreeIndex { Name = "Idx_Name" };

        _table.AddIndex(index);

        _table.Indexes.Should().Contain(index);
    }

    [Fact]
    public void RemoveIndex_ShouldRemoveIndex()
    {
        var index = new BTreeIndex { Name = "Idx_Name" };
        _table.AddIndex(index);

        _table.RemoveIndex("Idx_Name");

        _table.Indexes.Should().NotContain(index);
    }

    [Fact]
    public void AddTrigger_ShouldRegisterTrigger()
    {
        var trigger = new Trigger { Name = "Trg_BeforeInsert" };

        _table.AddTrigger(trigger);

        _table.Triggers.Should().Contain(trigger);
    }

    [Fact]
    public void RemoveTrigger_ShouldRemoveTrigger()
    {
        var trigger = new Trigger { Name = "Trg_BeforeInsert" };
        _table.AddTrigger(trigger);

        _table.RemoveTrigger("Trg_BeforeInsert");

        _table.Triggers.Should().NotContain(trigger);
    }
}
