using System;
using DBMS.Domain.Catalog.Builder;
using DBMS.Domain.Catalog.Composite;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Catalog;

public class TableBuilderTests
{
    [Fact]
    public void Reset_ShouldThrowNotImplementedException()
    {
        var builder = new TableBuilder();
        Action act = () => builder.Reset("Users");
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void AddColumn_ShouldThrowNotImplementedException()
    {
        var builder = new TableBuilder();
        var col = new Column { Name = "Id" };
        Action act = () => builder.AddColumn(col);
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Build_ShouldThrowNotImplementedException()
    {
        var builder = new TableBuilder();
        Action act = () => builder.Build();
        act.Should().Throw<NotImplementedException>();
    }
}
