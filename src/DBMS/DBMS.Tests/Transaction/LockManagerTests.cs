using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.Transaction;
using DBMS.Domain.Models;

namespace DBMS.Tests.Transaction;

public class LockManagerTests
{
    private readonly LockManager _manager;

    public LockManagerTests()
    {
        var detector = new DeadlockDetector();
        _manager = new LockManager(detector);
    }

    [Fact]
    public void AcquireLock_ShouldAcquireLockOnResource()
    {
        // Arrange
        var txId = new TransactionId(1);
        var resId = new ResourceId();
        var mode = new LockMode();

        // Act
        var act = () => _manager.AcquireLock(txId, resId, mode);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ReleaseLock_ShouldReleaseLockOnResource()
    {
        // Arrange
        var txId = new TransactionId(1);
        var resId = new ResourceId();

        // Act
        var act = () => _manager.ReleaseLock(txId, resId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ReleaseAll_ShouldReleaseAllLocksForTransaction()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _manager.ReleaseAll(txId);

        // Assert
        act.Should().NotThrow();
    }
}
