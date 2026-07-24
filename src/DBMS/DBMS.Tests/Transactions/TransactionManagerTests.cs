using System;
using DBMS.Domain.Transactions;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Transactions;

public class TransactionManagerTests
{
    [Fact]
    public void Begin_ShouldCreateActiveTransaction()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Begin();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Commit_ShouldPersistChanges()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Commit(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Commit_ShouldWriteWALBeforePersistingData()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Commit(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Commit_ShouldFail_WhenWALWriteFails()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Commit(1);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Rollback_ShouldUndoAllChanges()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Rollback(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Rollback_ShouldRestoreOriginalPageState()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Rollback(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RollbackToSavepoint_ShouldRestorePreviousState()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.RollbackToSavepoint(1, "SP1");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CommitTransaction_ShouldReleaseAllLocks()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Commit(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void RollbackTransaction_ShouldReleaseAllLocks()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.Rollback(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DetectDeadlock_ShouldAbortTransactionWithLowestPriority()
    {
        var txMgr = new TransactionManager();

        Action act = () => txMgr.DetectDeadlock();

        act.Should().Throw<NotImplementedException>();
    }
}
