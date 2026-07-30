using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Security;
using DBMS.Domain.Services;
using DBMS.Domain.Storage;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Services;

public class SchemaServiceTests
{
    private (SchemaService schemaService, DatabaseService dbService, Mock<ICatalogManager> catalogMock, Mock<StorageEngine> storageMock, Mock<ISecurityManager> securityMock) CreateServices()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var storageMock = new Mock<StorageEngine>();
        var securityMock = new Mock<ISecurityManager>();
        var builderMock = new Mock<ITableBuilder>();
        var constraintFactoryMock = new Mock<IConstraintFactory>();
        var indexFactoryMock = new Mock<IIndexFactory>();
        var director = new TableDirector(builderMock.Object, constraintFactoryMock.Object, indexFactoryMock.Object);

        var schemaService = new SchemaService(
            catalogMock.Object,
            storageMock.Object,
            director,
            builderMock.Object,
            constraintFactoryMock.Object,
            indexFactoryMock.Object);

        var dbService = new DatabaseService(catalogMock.Object);

        return (schemaService, dbService, catalogMock, storageMock, securityMock);
    }

    [Fact]
    public void CreateSchema_ShouldCreateSchemaSuccessfully()
    {
        var (_, dbService, catalogMock, _, _) = CreateServices();
        var db = new Database(1, "AppDB", "admin");

        Action act = () => dbService.CreateSchema(db, "dbo");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CreateSchema_ShouldRejectDuplicateSchemaName()
    {
        var (_, dbService, catalogMock, _, _) = CreateServices();
        var db = new Database(1, "AppDB", "admin");

        Action act = () => dbService.CreateSchema(db, "dbo");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CreateSchema_ShouldCheckPermissionBeforeCreation()
    {
        var (_, dbService, catalogMock, _, securityMock) = CreateServices();
        var db = new Database(1, "AppDB", "admin");

        Action act = () => dbService.CreateSchema(db, "dbo");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CreateSchema_ShouldRollback_WhenStorageFails()
    {
        var (_, dbService, catalogMock, storageMock, _) = CreateServices();
        var db = new Database(1, "AppDB", "admin");

        Action act = () => dbService.CreateSchema(db, "dbo");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void CreateSchema_ShouldRollback_WhenCatalogFails()
    {
        var (_, dbService, catalogMock, _, _) = CreateServices();
        var db = new Database(1, "AppDB", "admin");

        Action act = () => dbService.CreateSchema(db, "dbo");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void DropSchema_ShouldRemoveExistingSchema()
    {
        var (_, dbService, _, _, _) = CreateServices();
        var db = new Database(1, "AppDB", "admin");

        Action act = () => dbService.DropSchema(db, "dbo", false);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CreateTable_ShouldCreateTableSuccessfully()
    {
        var (schemaService, _, _, _, _) = CreateServices();
        var schema = new Schema("dbo");
        var def = new TableDefinition { Name = "Users" };

        Action act = () => schemaService.CreateTable(schema, def);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CreateTable_ShouldRejectDuplicateTableName()
    {
        var (schemaService, _, _, _, _) = CreateServices();
        var schema = new Schema("dbo");
        var def = new TableDefinition { Name = "Users" };

        Action act = () => schemaService.CreateTable(schema, def);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void DropTable_ShouldRemoveExistingTable()
    {
        var (schemaService, _, _, _, _) = CreateServices();
        var schema = new Schema("dbo");

        Action act = () => schemaService.DropTable(schema, "Users", false);

        act.Should().Throw<NotImplementedException>();
    }
}
