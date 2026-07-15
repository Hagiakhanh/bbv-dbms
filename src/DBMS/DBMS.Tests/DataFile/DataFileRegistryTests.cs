using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using DBMS.Domain.DataFile;
using DBMS.Domain.Foundation;

namespace DBMS.Tests.DataFile;

public class DataFileRegistryTests
{
    [Fact]
    public void RegisterFile_ValidPath_ReturnsNonNegativeId()
    {
        var registry = new DataFileRegistry();
        int fileId = registry.RegisterFile("a.db", FileType.Data);
        fileId.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void RegisterFile_TwoFiles_IdsAreDistinct()
    {
        var registry = new DataFileRegistry();
        int id1 = registry.RegisterFile("a.db", FileType.Data);
        int id2 = registry.RegisterFile("b.db", FileType.Data);
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void RegisterFile_IdsAreMonotonicallyIncreasing()
    {
        var registry = new DataFileRegistry();
        var ids = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            ids.Add(registry.RegisterFile($"file{i}.db", FileType.Data));
        }
        
        for (int i = 0; i < 4; i++)
        {
            ids[i].Should().BeLessThan(ids[i + 1]);
        }
    }

    [Fact]
    public void RegisterFile_NullPath_ThrowsArgumentNullException()
    {
        var registry = new DataFileRegistry();
        Action act = () => registry.RegisterFile(null!, FileType.Data);
        act.Should().Throw<ArgumentNullException>().WithParameterName("path");
    }

    [Fact]
    public void RegisterFile_EmptyPath_ThrowsArgumentException()
    {
        var registry = new DataFileRegistry();
        Action act = () => registry.RegisterFile("", FileType.Data);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RegisterFile_SamePathTwice_ThrowsInvalidOperationException()
    {
        var registry = new DataFileRegistry();
        registry.RegisterFile("a.db", FileType.Data);
        Action act = () => registry.RegisterFile("a.db", FileType.Data);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RegisterFile_DifferentFileTypes_BothSucceed()
    {
        var registry = new DataFileRegistry();
        int id1 = registry.RegisterFile("a.db", FileType.Data);
        int id2 = registry.RegisterFile("b.db", FileType.Log);
        id1.Should().NotBe(id2);
    }

    [Fact]
    public void RegisterFile_WhitespaceOnlyPath_ThrowsArgumentException()
    {
        var registry = new DataFileRegistry();
        Action act = () => registry.RegisterFile("   ", FileType.Data);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsRegistered_AfterRegister_ReturnsTrue()
    {
        var registry = new DataFileRegistry();
        int id = registry.RegisterFile("a.db", FileType.Data);
        registry.IsRegistered(id).Should().BeTrue();
    }

    [Fact]
    public void IsRegistered_BeforeRegister_ReturnsFalse()
    {
        var registry = new DataFileRegistry();
        registry.IsRegistered(999).Should().BeFalse();
    }

    [Fact]
    public void IsRegistered_AfterUnregister_ReturnsFalse()
    {
        var registry = new DataFileRegistry();
        int id = registry.RegisterFile("a.db", FileType.Data);
        registry.UnregisterFile(id);
        registry.IsRegistered(id).Should().BeFalse();
    }

    [Fact]
    public void GetFilePath_RegisteredId_ReturnsCorrectPath()
    {
        var registry = new DataFileRegistry();
        int id = registry.RegisterFile("data/t.db", FileType.Data);
        registry.GetFilePath(id).Should().Be("data/t.db");
    }

    [Fact]
    public void GetFilePath_UnknownId_ThrowsKeyNotFoundException()
    {
        var registry = new DataFileRegistry();
        Action act = () => registry.GetFilePath(999);
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void GetFilePath_AfterUnregister_ThrowsKeyNotFoundException()
    {
        var registry = new DataFileRegistry();
        int id = registry.RegisterFile("a.db", FileType.Data);
        registry.UnregisterFile(id);
        Action act = () => registry.GetFilePath(id);
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void UnregisterFile_ExistingId_Succeeds()
    {
        var registry = new DataFileRegistry();
        int id = registry.RegisterFile("a.db", FileType.Data);
        registry.UnregisterFile(id);
        // Succeeds if no exception
    }

    [Fact]
    public void UnregisterFile_UnknownId_ThrowsKeyNotFoundException()
    {
        var registry = new DataFileRegistry();
        Action act = () => registry.UnregisterFile(999);
        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void UnregisterFile_TwiceSameId_ThrowsOnSecondCall()
    {
        var registry = new DataFileRegistry();
        int id = registry.RegisterFile("a.db", FileType.Data);
        registry.UnregisterFile(id);
        Action act = () => registry.UnregisterFile(id);
        act.Should().Throw<Exception>(); // Specifically what exception? usually KeyNotFound or InvalidOp
    }

    [Fact]
    public void RegisterAfterUnregister_NewIdAssigned()
    {
        var registry = new DataFileRegistry();
        int id1 = registry.RegisterFile("a.db", FileType.Data);
        registry.UnregisterFile(id1);
        int id2 = registry.RegisterFile("a.db", FileType.Data);
        id2.Should().NotBe(id1);
    }
}
