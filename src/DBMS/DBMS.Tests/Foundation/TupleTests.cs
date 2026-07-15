using System;
using FluentAssertions;
using Xunit;
using DBMS.Domain.Foundation;

namespace DBMS.Tests.Foundation;

public class TupleTests
{
    [Fact]
    public void Constructor_ValidData_Succeeds()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        
        // Act
        var tuple = new DBMS.Domain.Foundation.Tuple(data);
        
        // Assert
        tuple.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_NullData_ThrowsArgumentNullException()
    {
        // Arrange & Act
        Action act = () => new DBMS.Domain.Foundation.Tuple(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("data");
    }

    [Fact]
    public void Constructor_EmptyArray_SizeIsZero()
    {
        // Arrange
        var tuple = new DBMS.Domain.Foundation.Tuple(Array.Empty<byte>());
        
        // Act & Assert
        tuple.Size.Should().Be(0);
    }

    [Fact]
    public void Size_ReturnsDataLength()
    {
        // Arrange
        var tuple = new DBMS.Domain.Foundation.Tuple(new byte[100]);
        
        // Act & Assert
        tuple.Size.Should().Be(100);
    }

    [Fact]
    public void Size_IsReadOnly_NoPublicSetter()
    {
        // Arrange & Act
        var property = typeof(DBMS.Domain.Foundation.Tuple).GetProperty("Size");
        
        // Assert
        property.Should().NotBeNull();
        bool hasPublicSetter = property!.SetMethod != null && property.SetMethod.IsPublic;
        hasPublicSetter.Should().BeFalse();
    }

    [Fact]
    public void GetData_ReturnsOriginalBytes()
    {
        // Arrange
        var data = new byte[] { 0xAA, 0xBB };
        var tuple = new DBMS.Domain.Foundation.Tuple(data);
        
        // Act & Assert
        tuple.GetData().Should().BeEquivalentTo(new byte[] { 0xAA, 0xBB });
    }

    [Fact]
    public void GetData_ReturnsDefensiveCopy_DifferentReference()
    {
        // Arrange
        var data = new byte[] { 0xAA, 0xBB };
        var tuple = new DBMS.Domain.Foundation.Tuple(data);
        
        // Act
        var call1 = tuple.GetData();
        var call2 = tuple.GetData();
        
        // Assert
        ReferenceEquals(call1, call2).Should().BeFalse();
    }

    [Fact]
    public void GetData_MutatingResult_DoesNotAffectTuple()
    {
        // Arrange
        var data = new byte[] { 0xAA, 0xBB };
        var tuple = new DBMS.Domain.Foundation.Tuple(data);
        
        // Act
        var d = tuple.GetData();
        d[0] = 0xFF;
        
        // Assert
        tuple.GetData()[0].Should().Be(0xAA);
    }

    [Fact]
    public void Rid_DefaultValue_IsZeroZero()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        var tuple = new DBMS.Domain.Foundation.Tuple(data);
        
        // Act & Assert
        tuple.Rid.Should().Be(new RID(0, 0));
    }

    [Fact]
    public void Rid_CanBeAssigned()
    {
        // Arrange
        var tuple = new DBMS.Domain.Foundation.Tuple(new byte[] { 1, 2, 3 });
        
        // Act
        tuple.Rid = new RID(5, 2);
        
        // Assert
        tuple.Rid.Should().Be(new RID(5, 2));
    }

    [Fact]
    public void Rid_CanBeReassigned()
    {
        // Arrange
        var tuple = new DBMS.Domain.Foundation.Tuple(new byte[] { 1, 2, 3 });
        
        // Act
        tuple.Rid = new RID(1, 1);
        tuple.Rid = new RID(5, 2);
        
        // Assert
        tuple.Rid.Should().Be(new RID(5, 2));
    }
}
