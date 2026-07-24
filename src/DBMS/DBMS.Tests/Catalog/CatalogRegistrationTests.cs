using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Catalog.Composite;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Catalog;

public class CatalogRegistrationTests
{
    [Fact]
    public void RegisterDatabase_ShouldAddDatabaseMetadata()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.RegisterDatabase("NewDB");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RegisterDatabase_ShouldRejectDuplicateDatabase()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.RegisterDatabase("NewDB");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void RegisterSchema_ShouldAddSchemaMetadata()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.RegisterSchema("AppDB", "dbo");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RegisterSchema_ShouldRejectDuplicateSchema()
    {
        var catalog = new CatalogManager();

        Action act = () => catalog.RegisterSchema("AppDB", "dbo");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void RegisterTable_ShouldAddTableMetadata()
    {
        var catalog = new CatalogManager();
        var table = new Table("Users");

        Action act = () => catalog.RegisterTable(table);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RegisterTable_ShouldRejectDuplicateTable()
    {
        var catalog = new CatalogManager();
        var table = new Table("Users");

        Action act = () => catalog.RegisterTable(table);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void RegisterTable_ShouldRollback_WhenStorageFails()
    {
        var catalog = new CatalogManager();
        var table = new Table("Users");

        Action act = () => catalog.RegisterTable(table);

        act.Should().Throw<Exception>();
    }
}
