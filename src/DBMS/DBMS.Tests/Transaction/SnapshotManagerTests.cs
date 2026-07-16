using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.Transaction;
using DBMS.Domain.Models;

namespace DBMS.Tests.Transaction;

public class SnapshotManagerTests
{
    private readonly SnapshotManager _manager;

    public SnapshotManagerTests()
    {
        _manager = new SnapshotManager();
    }

    [Fact]
    public void CreateSnapshot_ShouldCreateNewSnapshot()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _manager.CreateSnapshot(txId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ReleaseSnapshot_ShouldReleaseSnapshot()
    {
        // Arrange
        var snapshotId = new SnapshotId();

        // Act
        var act = () => _manager.ReleaseSnapshot(snapshotId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void IsVisible_ShouldReturnTrueIfVisible()
    {
        // Arrange
        var txId = new TransactionId(1);
        var snapshotId = new SnapshotId();

        // Act
        var act = () => _manager.IsVisible(txId, snapshotId);

        // Assert
        act.Should().NotThrow();
    }
}
