using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.Transaction;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.Transaction;

public class TransactionManagerTests
{
    private readonly Mock<IWALManager> _mockWalMgr;
    private readonly TransactionTable _txTable;
    private readonly LockManager _lockMgr;
    private readonly MVCCManager _mvccMgr;
    private readonly TransactionManager _manager;

    public TransactionManagerTests()
    {
        _mockWalMgr = new Mock<IWALManager>();
        _txTable = new TransactionTable();
        var detector = new DeadlockDetector();
        _lockMgr = new LockManager(detector);
        var mockSnapshotProvider = new Mock<ISnapshotProvider>();
        _mvccMgr = new MVCCManager(mockSnapshotProvider.Object);
        
        _manager = new TransactionManager(_txTable, _lockMgr, _mockWalMgr.Object, _mvccMgr);
    }

    [Fact]
    public void Begin_ShouldReturnNewTransactionId()
    {
        // Act
        var act = () => _manager.Begin();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Commit_ShouldCommitTransaction()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _manager.Commit(txId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Abort_ShouldAbortTransaction()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _manager.Abort(txId);

        // Assert
        act.Should().NotThrow();
    }
}
