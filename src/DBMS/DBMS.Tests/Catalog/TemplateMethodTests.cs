using Xunit;
using FluentAssertions;
using DBMS.Domain.Catalog.Composite;
using DBMS.Domain.Catalog.Template;
using DBMS.Domain.Catalog.Strategy;

namespace DBMS.Tests.Catalog;

public class TemplateMethodTests
{
    [Fact]
    public void CreateTableScriptGenerator_ShouldGenerateValidDdl()
    {
        // Arrange
        var schema = new Schema("dbo");
        var table = new Table("Users") { Parent = schema };
        
        table.Columns.AsListIfPossible(); // Check columns structure

        var colId = new Column
        {
            Name = "Id",
            DataType = DBMS.Domain.Core.DataTypeEnum.INT,
            Nullable = false
        };
        var colName = new Column
        {
            Name = "Username",
            DataType = DBMS.Domain.Core.DataTypeEnum.VARCHAR,
            Nullable = true,
            DefaultValue = "'guest'"
        };

        // Access via reflection or direct list addition if needed
        var columnsField = typeof(Table).GetField("_columns", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var colsList = (System.Collections.Generic.List<Column>)columnsField!.GetValue(table)!;
        colsList.Add(colId);
        colsList.Add(colName);

        var generator = new CreateTableScriptGenerator(table);

        // Act
        var script = generator.Generate();

        // Assert
        script.Should().Contain("CREATE TABLE dbo.Users (");
        script.Should().Contain("    Id INT NOT NULL");
        script.Should().Contain("    Username VARCHAR DEFAULT 'guest'");
        script.Should().EndWith(");");
    }

    [Fact]
    public void DropTableScriptGenerator_ShouldGenerateValidDdl()
    {
        // Arrange
        var generatorWithoutCascade = new DropTableScriptGenerator("Users", false);
        var generatorWithCascade = new DropTableScriptGenerator("Users", true);

        // Act
        var script1 = generatorWithoutCascade.Generate();
        var script2 = generatorWithCascade.Generate();

        // Assert
        script1.Should().Be("DROP TABLE IF EXISTS Users;");
        script2.Should().Be("DROP TABLE IF EXISTS Users CASCADE;");
    }

    [Fact]
    public void CreateSchemaScriptGenerator_ShouldGenerateValidDdl()
    {
        // Arrange
        var schema = new Schema("sales");
        var generator = new CreateSchemaScriptGenerator(schema);

        // Act
        var script = generator.Generate();

        // Assert
        script.Should().Be("CREATE SCHEMA sales;");
    }

    [Fact]
    public void AlterTableScriptGenerator_ShouldGenerateValidDdl()
    {
        // Arrange
        var schema = new Schema("public");
        var table = new Table("Orders") { Parent = schema };
        var op = new TableAlterOperation
        {
            Type = TableAlterType.ADD_COLUMN,
            Definition = "Total DECIMAL(10, 2)"
        };
        var generator = new AlterTableScriptGenerator(table, op);

        // Act
        var script = generator.Generate();

        // Assert
        script.Should().Contain("ALTER TABLE public.Orders");
        script.Should().Contain("    ADD_COLUMN Total DECIMAL(10, 2)");
        script.Should().EndWith(";");
    }
}

internal static class ListExtensions
{
    public static void AsListIfPossible(this object obj) { }
}
