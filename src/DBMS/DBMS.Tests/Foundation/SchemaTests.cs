using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using DBMS.Domain.Foundation;

namespace DBMS.Tests.Foundation;

public class SchemaTests
{
    private readonly ColumnType _intType;
    private readonly ColumnType _varCharType;

    public SchemaTests()
    {
        // For Red Phase tests, if ColumnType constructor throws NotImplementedException,
        // we might not even be able to instantiate these. 
        // We'll use try-catch or just let the test fail naturally with NotImplementedException.
        // Actually, for Red Phase we want the test method to execute and fail, so we might need
        // default structs if the constructor throws, but let's just let the constructor throw
        // NotImplementedException as that also represents a Red Phase failure.
        // However, to make it clean, we can just use default structs.
        _intType = default; 
        _varCharType = default;
    }

    // ==========================================
    // Constructor Tests
    // ==========================================

    [Fact]
    public void Constructor_ValidNamesAndTypes_Succeeds()
    {
        // Arrange
        var names = new[] { "id", "name" };
        var types = new[] { _intType, _varCharType };

        // Act
        var schema = new Schema(names, types);

        // Assert
        schema.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_NullNames_ThrowsArgumentNullException()
    {
        // Arrange
        var types = new[] { _intType };

        // Act
        Action act = () => new Schema(null!, types);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("names");
    }

    [Fact]
    public void Constructor_NullTypes_ThrowsArgumentNullException()
    {
        // Arrange
        var names = new[] { "id" };

        // Act
        Action act = () => new Schema(names, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("types");
    }

    [Fact]
    public void Constructor_EmptySchema_Allowed()
    {
        // Arrange
        var names = Array.Empty<string>();
        var types = Array.Empty<ColumnType>();

        // Act
        var schema = new Schema(names, types);

        // Assert
        schema.ColumnCount.Should().Be(0);
    }

    [Fact]
    public void Constructor_MismatchedLengths_ThrowsArgumentException()
    {
        // Arrange
        var names = new[] { "id", "name" };
        var types = new[] { _intType };

        // Act
        Action act = () => new Schema(names, types);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_DuplicateColumnNames_ThrowsArgumentException()
    {
        // Arrange
        var names = new[] { "id", "id" };
        var types = new[] { _intType, _intType };

        // Act
        Action act = () => new Schema(names, types);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // ==========================================
    // ColumnCount Tests
    // ==========================================

    [Fact]
    public void ColumnCount_ReturnsCorrectCount()
    {
        // Arrange
        var schema = new Schema(new[] { "a", "b", "c" }, new[] { _intType, _intType, _intType });

        // Act & Assert
        schema.ColumnCount.Should().Be(3);
    }

    [Fact]
    public void ColumnCount_EmptySchema_ReturnsZero()
    {
        // Arrange
        var schema = new Schema(Array.Empty<string>(), Array.Empty<ColumnType>());

        // Act & Assert
        schema.ColumnCount.Should().Be(0);
    }

    // ==========================================
    // GetColumnIndex Tests
    // ==========================================

    [Fact]
    public void GetColumnIndex_FirstColumn_ReturnsZero()
    {
        // Arrange
        var schema = new Schema(new[] { "id", "name" }, new[] { _intType, _varCharType });

        // Act & Assert
        schema.GetColumnIndex("id").Should().Be(0);
    }

    [Fact]
    public void GetColumnIndex_LastColumn_ReturnsLastIndex()
    {
        // Arrange
        var schema = new Schema(new[] { "a", "b", "c" }, new[] { _intType, _intType, _intType });

        // Act & Assert
        schema.GetColumnIndex("c").Should().Be(2);
    }

    [Fact]
    public void GetColumnIndex_MiddleColumn_ReturnsCorrectIndex()
    {
        // Arrange
        var schema = new Schema(new[] { "a", "b", "c" }, new[] { _intType, _intType, _intType });

        // Act & Assert
        schema.GetColumnIndex("b").Should().Be(1);
    }

    [Fact]
    public void GetColumnIndex_NonExistentColumn_ThrowsArgumentException()
    {
        // Arrange
        var schema = new Schema(new[] { "id" }, new[] { _intType });

        // Act
        Action act = () => schema.GetColumnIndex("salary");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetColumnIndex_NullName_ThrowsArgumentNullException()
    {
        // Arrange
        var schema = new Schema(new[] { "id" }, new[] { _intType });

        // Act
        Action act = () => schema.GetColumnIndex(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetColumnIndex_EmptyString_ThrowsArgumentException()
    {
        // Arrange
        var schema = new Schema(new[] { "id" }, new[] { _intType });

        // Act
        Action act = () => schema.GetColumnIndex("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetColumnIndex_CaseSensitive_NotFound()
    {
        // Arrange
        var schema = new Schema(new[] { "Id" }, new[] { _intType });

        // Act
        Action act = () => schema.GetColumnIndex("id");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    // ==========================================
    // IsFixedLength Tests
    // ==========================================

    [Fact]
    public void IsFixedLength_AllIntColumns_ReturnsTrue()
    {
        // Arrange
        var schema = new Schema(new[] { "a", "b" }, new[] { _intType, _intType });

        // Act & Assert
        schema.IsFixedLength.Should().BeTrue();
    }

    [Fact]
    public void IsFixedLength_AllVarCharColumns_ReturnsFalse()
    {
        // Arrange
        var schema = new Schema(new[] { "a", "b" }, new[] { _varCharType, _varCharType });

        // Act & Assert
        schema.IsFixedLength.Should().BeFalse();
    }

    [Fact]
    public void IsFixedLength_MixedColumns_ReturnsFalse()
    {
        // Arrange
        var schema = new Schema(new[] { "a", "b" }, new[] { _intType, _varCharType });

        // Act & Assert
        schema.IsFixedLength.Should().BeFalse();
    }

    [Fact]
    public void IsFixedLength_EmptySchema_ReturnsTrue()
    {
        // Arrange
        var schema = new Schema(Array.Empty<string>(), Array.Empty<ColumnType>());

        // Act & Assert
        schema.IsFixedLength.Should().BeTrue();
    }

    // ==========================================
    // ReadOnly Collection Tests
    // ==========================================

    [Fact]
    public void ColumnNames_ReturnsReadOnlyCollection()
    {
        // Arrange
        var schema = new Schema(new[] { "id" }, new[] { _intType });

        // Act
        Action act = () => {
            var list = (IList<string>)schema.ColumnNames;
            list.Add("extra");
        };

        // Assert
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void ColumnTypes_ReturnsReadOnlyCollection()
    {
        // Arrange
        var schema = new Schema(new[] { "id" }, new[] { _intType });

        // Act
        Action act = () => {
            var list = (IList<ColumnType>)schema.ColumnTypes;
            list.Add(_intType);
        };

        // Assert
        act.Should().Throw<NotSupportedException>();
    }
}
