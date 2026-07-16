using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.Transaction;

namespace DBMS.Tests.Transaction;

public class DeadlockDetectorTests
{
    private readonly DeadlockDetector _detector;

    public DeadlockDetectorTests()
    {
        _detector = new DeadlockDetector();
    }

    [Fact]
    public void Check_ShouldCheckForDeadlocks()
    {
        // Arrange
        var txTable = new TransactionTable();

        // Act
        var act = () => _detector.Check(txTable);

        // Assert
        act.Should().NotThrow();
    }
}
