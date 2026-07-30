using System;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Catalog;

public class MetadataVisitorTests
{
    [Fact]
    public void DdlExportVisitor_VisitTable_ShouldThrowNotImplementedException()
    {
        var visitor = new DdlExportVisitor();
        var table = new Table("Users");

        Action act = () => visitor.VisitTable(table);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DdlExportVisitor_GetResult_ShouldReturnString()
    {
        var visitor = new DdlExportVisitor();
        var result = visitor.GetResult();

        result.Should().BeEmpty();
    }

    [Fact]
    public void DependencyScanVisitor_VisitSchema_ShouldThrowNotImplementedException()
    {
        var visitor = new DependencyScanVisitor();
        var schema = new Schema("dbo");

        Action act = () => visitor.VisitSchema(schema);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DependencyScanVisitor_GetDependencies_ShouldReturnReadOnlyCollection()
    {
        var visitor = new DependencyScanVisitor();
        var dependencies = visitor.GetDependencies();

        dependencies.Should().BeEmpty();
    }
}
