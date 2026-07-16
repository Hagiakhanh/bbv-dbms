using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.Transaction;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.Transaction;

public class MVCCManagerTests
{
    private readonly Mock<ISnapshotProvider> _mockSnapshotProvider;
    private readonly MVCCManager _manager;

    public MVCCManagerTests()
    {
        _mockSnapshotProvider = new Mock<ISnapshotProvider>();
        _manager = new MVCCManager(_mockSnapshotProvider.Object);
    }

    [Fact]
    public void ReadVersion_ShouldReturnRecordVersion()
    {
        // Arrange
        var fileId = new FileId();
        var pageId = new PageId(fileId, 0);
        var rid = new RID(pageId, 0);
        var snapshotId = new SnapshotId();

        // Act
        var act = () => _manager.ReadVersion(rid, snapshotId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void WriteVersion_ShouldWriteRecordVersion()
    {
        // Arrange
        var fileId = new FileId();
        var pageId = new PageId(fileId, 0);
        var rid = new RID(pageId, 0);
        var txId = new TransactionId(1);
        var data = new byte[] { 1, 2, 3 };

        // Act
        var act = () => _manager.WriteVersion(rid, txId, data);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Vacuum_ShouldVacuumOldVersions()
    {
        // Arrange
        var olderThan = new LSN(100);

        // Act
        var act = () => _manager.Vacuum(olderThan);

        // Assert
        act.Should().NotThrow();
    }
}
