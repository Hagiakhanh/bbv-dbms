using System;
using DBMS.Domain.Core;
using DBMS.Domain.Storage;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Storage;

public class PageAndFileManagerTests
{
    [Fact]
    public void InsertRecord_ShouldReject_WhenPageIsFull()
    {
        var page = new Page();
        var record = new byte[4096];

        Action act = () => page.InsertRecord(record);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DeleteRecord_ShouldRemoveRecord()
    {
        var page = new Page();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => page.DeleteRecord(rid);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Compact_ShouldReclaimFreeSpace()
    {
        var page = new Page();

        Action act = () => page.Compact();

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ReadPage_ShouldReturnRequestedPage()
    {
        var storage = new StorageEngine();

        Action act = () => storage.ReadPage(1);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void WritePage_ShouldPersistPageData()
    {
        var storage = new StorageEngine();
        var data = new byte[512];

        Action act = () => storage.WritePage(1, data);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void AllocatePage_ShouldExtendFile_WhenNoFreePagesExist()
    {
        var storage = new StorageEngine();

        Action act = () => storage.AllocatePage(100);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void DeleteRecord_ShouldUpdateFreeSpaceCorrectly()
    {
        var page = new Page();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => page.DeleteRecord(rid);

        act.Should().Throw<NotImplementedException>();
    }
}
