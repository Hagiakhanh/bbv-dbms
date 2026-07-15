using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using DBMS.Domain.Foundation;

namespace DBMS.Tests.Foundation;

public class RIDTests
{
    // ==========================================
    // Constructor Tests
    // ==========================================

    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        // Arrange & Act
        var rid = new RID(3, 7);

        // Assert
        rid.PageId.Should().Be(3);
        rid.SlotId.Should().Be(7);
    }

    [Fact]
    public void Constructor_ZeroValues_IsAllowed()
    {
        // Arrange & Act
        var rid = new RID(0, 0);

        // Assert
        rid.PageId.Should().Be(0);
        rid.SlotId.Should().Be(0);
    }

    [Fact]
    public void Constructor_MaxIntValues_DoesNotOverflow()
    {
        // Arrange & Act
        var rid = new RID(int.MaxValue, int.MaxValue);

        // Assert
        rid.PageId.Should().Be(2147483647);
        rid.SlotId.Should().Be(2147483647);
    }

    [Fact]
    public void Constructor_NegativePageId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange & Act
        Action act = () => new RID(-1, 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("pageId");
    }

    [Fact]
    public void Constructor_NegativeSlotId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange & Act
        Action act = () => new RID(0, -1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("slotId");
    }

    // ==========================================
    // Equality Tests
    // ==========================================

    [Fact]
    public void Equals_SamePageAndSlot_ReturnsTrue()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(3, 7);

        // Act & Assert
        rid1.Equals(rid2).Should().BeTrue();
        rid1.Equals((object)rid2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentPageId_ReturnsFalse()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(4, 7);

        // Act & Assert
        rid1.Equals(rid2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentSlotId_ReturnsFalse()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(3, 8);

        // Act & Assert
        rid1.Equals(rid2).Should().BeFalse();
    }

    [Fact]
    public void Equals_BothDifferent_ReturnsFalse()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(4, 8);

        // Act & Assert
        rid1.Equals(rid2).Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var rid = new RID(3, 7);

        // Act & Assert
        rid.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        // Arrange
        var rid = new RID(3, 7);

        // Act & Assert
        rid.Equals("(3:7)").Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_SameValues_ReturnsTrue()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(3, 7);

        // Act & Assert
        (rid1 == rid2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_DifferentValues_ReturnsTrue()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(4, 7);

        // Act & Assert
        (rid1 != rid2).Should().BeTrue();
    }

    // ==========================================
    // GetHashCode Tests
    // ==========================================

    [Fact]
    public void GetHashCode_EqualRIDs_SameHash()
    {
        // Arrange
        var rid1 = new RID(3, 7);
        var rid2 = new RID(3, 7);

        // Act & Assert
        rid1.GetHashCode().Should().Be(rid2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentRIDs_DifferentHash()
    {
        // Arrange
        var rid1 = new RID(1, 2);
        var rid2 = new RID(3, 4);

        // Act & Assert
        rid1.GetHashCode().Should().NotBe(rid2.GetHashCode());
    }

    // ==========================================
    // ToString Tests
    // ==========================================

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var rid = new RID(3, 7);

        // Act & Assert
        rid.ToString().Should().Be("(3:7)");
    }

    [Fact]
    public void ToString_ZeroValues_ReturnsCorrectFormat()
    {
        // Arrange
        var rid = new RID(0, 0);

        // Act & Assert
        rid.ToString().Should().Be("(0:0)");
    }

    // ==========================================
    // Misc Tests
    // ==========================================

    [Fact]
    public void Default_BothPropertiesAreZero()
    {
        // Arrange
        RID rid = default;

        // Act & Assert
        rid.PageId.Should().Be(0);
        rid.SlotId.Should().Be(0);
    }

    [Fact]
    public void UsableAsDictionaryKey_Works()
    {
        // Arrange
        var dict = new Dictionary<RID, string>();
        var rid = new RID(1, 2);
        var identicalRid = new RID(1, 2);

        // Act
        dict[rid] = "a";

        // Assert
        dict.ContainsKey(identicalRid).Should().BeTrue();
        dict[identicalRid].Should().Be("a");
    }
}
