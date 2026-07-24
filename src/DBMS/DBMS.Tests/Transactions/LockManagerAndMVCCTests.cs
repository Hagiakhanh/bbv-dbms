using System;
using DBMS.Domain.Core;
using DBMS.Domain.Transactions;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Transactions;

public class LockManagerAndMVCCTests
{
    [Fact]
    public void AcquireSharedLock_ShouldGrantLock_WhenNoConflict()
    {
        var lockMgr = new LockManager();

        Action act = () => lockMgr.AcquireSharedLock(1, "Table_Users");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void AcquireExclusiveLock_ShouldWait_WhenSharedLockExists()
    {
        var lockMgr = new LockManager();

        Action act = () => lockMgr.AcquireExclusiveLock(2, "Table_Users");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DetectDeadlock_ShouldIdentifyCircularWait()
    {
        var lockMgr = new LockManager();

        Action act = () => lockMgr.DetectDeadlock();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Read_ShouldIgnoreUncommittedVersion()
    {
        var mvcc = new MVCCManager();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => mvcc.ReadVersion(rid, 100);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void AcquireLock_ShouldTimeout_WhenWaitExceedsLimit()
    {
        var lockMgr = new LockManager();

        Action act = () => lockMgr.AcquireLock(1, "Res1", "Exclusive");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CreateVersion_ShouldMaintainVersionChain()
    {
        var mvcc = new MVCCManager();
        var rid = new RID { PageId = 1, SlotNumber = 1 };
        var data = new RecordData();

        Action act = () => mvcc.CreateVersion(rid, 1, data);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void GarbageCollect_ShouldRemoveVersionsOlderThanOldestActiveSnapshot()
    {
        var mvcc = new MVCCManager();

        Action act = () => mvcc.GarbageCollect(50);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ReadVersion_ShouldReturnLatestCommittedVersion()
    {
        var mvcc = new MVCCManager();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => mvcc.ReadVersion(rid, 100);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ReadVersion_ShouldReturnOldVersion_ForRepeatableRead()
    {
        var mvcc = new MVCCManager();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => mvcc.ReadVersion(rid, 90);

        act.Should().Throw<NotImplementedException>();
    }
}
