using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Catalog.Factory;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Security;
using DBMS.Domain.Storage;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Catalog;

public class DatabaseFactoryTests
{
    private (DatabaseFactory factory, Mock<ICatalogManager> catalog, Mock<IStorageEngine> storage, Mock<ISecurityManager> security) CreateFactory()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var storageMock = new Mock<IStorageEngine>();
        var securityMock = new Mock<ISecurityManager>();

        var factory = new DatabaseFactory(catalogMock.Object, storageMock.Object, securityMock.Object);
        return (factory, catalogMock, storageMock, securityMock);
    }

    [Fact]
    public void Create_ShouldInstantiateDatabaseAndRegisterDependencies()
    {
        var (factory, catalog, storage, security) = CreateFactory();
        var options = new DatabaseCreationOptions { Name = "FactoryTestDb", Owner = "admin" };

        var db = factory.Create(options);

        db.Should().NotBeNull();
        db.Name.Should().Be("FactoryTestDb");
        db.Owner.Should().Be("admin");

        storage.Verify(s => s.AllocateDatabase("FactoryTestDb"), Times.Once);
        catalog.Verify(c => c.RegisterDatabase("FactoryTestDb"), Times.Once);
        security.Verify(s => s.GrantOwnership("FactoryTestDb", "admin"), Times.Once);
    }

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        var (factory, _, _, _) = CreateFactory();

        Action act = () => factory.Create(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_ShouldThrowInvalidNameException_WhenDatabaseNameIsEmpty()
    {
        var (factory, _, _, _) = CreateFactory();
        var options = new DatabaseCreationOptions { Name = "" };

        Action act = () => factory.Create(options);

        act.Should().Throw<InvalidNameException>();
    }
}
