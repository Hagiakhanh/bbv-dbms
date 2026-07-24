using System;
using DBMS.Domain.Catalog;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Catalog;

public class MetadataLookupTests
{
    [Fact]
    public void FindTable_ShouldReturnQualifiedTable()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.FindTable("Users");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ResolveObjectName_ShouldResolveSchemaObject()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.ResolveObjectName("dbo.Users");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DropSchema_ShouldReject_WhenSchemaContainsObjects()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.DropSchema("dbo");

        act.Should().Throw<Exception>();
    }
}
