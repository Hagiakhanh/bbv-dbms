using System;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Iterator;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Catalog;

public class CatalogIteratorTests
{
    [Fact]
    public void CatalogIterator_HasMore_ShouldThrowNotImplementedException()
    {
        var table = new Table("Users");
        var iterator = new CatalogIterator(table);

        Action act = () => iterator.HasMore();
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CatalogIterator_GetNext_ShouldThrowNotImplementedException()
    {
        var table = new Table("Users");
        var iterator = new CatalogIterator(table);

        Action act = () => iterator.GetNext();
        act.Should().Throw<NotImplementedException>();
    }
}
