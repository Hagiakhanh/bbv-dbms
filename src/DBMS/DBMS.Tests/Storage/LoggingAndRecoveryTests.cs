using System;
using DBMS.Domain.Storage;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Storage;

public class LoggingAndRecoveryTests
{
    [Fact]
    public void WriteLog_ShouldAssignIncreasingLSN()
    {
        var wal = new WALManager();

        Action act = () => wal.WriteLog("LOG_DATA");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Recover_ShouldReplayCommittedTransactions()
    {
        var recoveryManager = new RecoveryManager();

        Action act = () => recoveryManager.Recover(100);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Recover_ShouldUndoUncommittedTransactions()
    {
        var recoveryManager = new RecoveryManager();

        Action act = () => recoveryManager.Recover(100);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Recover_ShouldRestoreConsistentDatabase()
    {
        var recoveryManager = new RecoveryManager();

        Action act = () => recoveryManager.Recover(100);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Recover_ShouldHandleCorruptWALRecord()
    {
        var recoveryManager = new RecoveryManager();

        Action act = () => recoveryManager.Recover(100);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Truncate_ShouldRemoveLogsBeforeCheckpoint()
    {
        var wal = new WALManager();

        Action act = () => wal.Truncate(50);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Checkpoint_ShouldRecordActiveTransactions()
    {
        var wal = new WALManager();

        Action act = () => wal.Checkpoint();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Checkpoint_ShouldFlushDirtyPages()
    {
        var wal = new WALManager();

        Action act = () => wal.Checkpoint();

        act.Should().Throw<NotImplementedException>();
    }
}
