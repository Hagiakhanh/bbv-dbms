using System;
using DBMS.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Catalog;

public class DdlCommandTests
{
    [Fact]
    public void CreateTableCommand_Execute_ShouldThrowNotImplementedException()
    {
        var schemaServiceMock = new Mock<ISchemaService>();
        var schema = new Schema("dbo");
        var def = new TableDefinition { Name = "Users" };
        var cmd = new CreateTableCommand(schemaServiceMock.Object, schema, def);

        Action act = () => cmd.Execute();
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DropTableCommand_Execute_ShouldThrowNotImplementedException()
    {
        var schemaServiceMock = new Mock<ISchemaService>();
        var schema = new Schema("dbo");
        var cmd = new DropTableCommand(schemaServiceMock.Object, schema, "Users");

        Action act = () => cmd.Execute();
        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DdlCommandExecutor_Execute_ShouldThrowNotImplementedException()
    {
        var executor = new DdlCommandExecutor();
        var cmdMock = new Mock<IDdlCommand>();

        Action act = () => executor.Execute(cmdMock.Object);
        act.Should().Throw<NotImplementedException>();
    }
}
