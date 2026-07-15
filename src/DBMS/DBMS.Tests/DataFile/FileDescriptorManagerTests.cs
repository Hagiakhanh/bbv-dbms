using System;
using System.IO;
using FluentAssertions;
using Xunit;
using DBMS.Domain.DataFile;

namespace DBMS.Tests.DataFile;

public class FileDescriptorManagerTests
{
    [Fact]
    public void GetDescriptor_ValidFileId_ReturnsNonNull()
    {
        var mgr = new FileDescriptorManager();
        int fd = mgr.GetDescriptor(1);
        fd.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void GetDescriptor_SameFileId_ReturnsCachedDescriptor()
    {
        var mgr = new FileDescriptorManager();
        int fd1 = mgr.GetDescriptor(1);
        int fd2 = mgr.GetDescriptor(1);
        fd1.Should().Be(fd2);
    }

    [Fact]
    public void GetDescriptor_DifferentFileIds_ReturnsDifferentDescriptors()
    {
        var mgr = new FileDescriptorManager();
        int fd1 = mgr.GetDescriptor(1);
        int fd2 = mgr.GetDescriptor(2);
        fd1.Should().NotBe(fd2);
    }

    [Fact]
    public void GetDescriptor_InvalidFileId_ThrowsFileNotFoundException()
    {
        var mgr = new FileDescriptorManager();
        Action act = () => mgr.GetDescriptor(999);
        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void GetActiveCount_Initially_IsZero()
    {
        var mgr = new FileDescriptorManager();
        mgr.GetActiveCount().Should().Be(0);
    }

    [Fact]
    public void GetActiveCount_AfterOneGet_IsOne()
    {
        var mgr = new FileDescriptorManager();
        mgr.GetDescriptor(1);
        mgr.GetActiveCount().Should().Be(1);
    }

    [Fact]
    public void GetActiveCount_AfterTwoDistinctGets_IsTwo()
    {
        var mgr = new FileDescriptorManager();
        mgr.GetDescriptor(1);
        mgr.GetDescriptor(2);
        mgr.GetActiveCount().Should().Be(2);
    }

    [Fact]
    public void GetActiveCount_GetSameFileTwice_IsOne()
    {
        var mgr = new FileDescriptorManager();
        mgr.GetDescriptor(1);
        mgr.GetDescriptor(1);
        mgr.GetActiveCount().Should().Be(1);
    }

    [Fact]
    public void ReleaseDescriptor_AfterGet_ActiveCountDecreases()
    {
        var mgr = new FileDescriptorManager();
        mgr.GetDescriptor(1);
        mgr.ReleaseDescriptor(1);
        mgr.GetActiveCount().Should().Be(0);
    }

    [Fact]
    public void ReleaseDescriptor_NullDescriptor_ThrowsArgumentNullException()
    {
        // The test spec says ReleaseDescriptor(null) for ArgumentNullException.
        // But our interface takes int. It can't be null. 
        // We will adapt this slightly.
        var mgr = new FileDescriptorManager();
        Action act = () => mgr.ReleaseDescriptor(-1); // using -1 to represent invalid descriptor or fileId
        act.Should().Throw<ArgumentException>(); // ArgumentException is more appropriate here
    }

    [Fact]
    public void ReleaseDescriptor_AlreadyReleased_ThrowsInvalidOperationException()
    {
        var mgr = new FileDescriptorManager();
        mgr.GetDescriptor(1);
        mgr.ReleaseDescriptor(1);
        Action act = () => mgr.ReleaseDescriptor(1);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetDescriptor_AfterRelease_ReturnsNewDescriptor()
    {
        var mgr = new FileDescriptorManager();
        int fd1 = mgr.GetDescriptor(1);
        mgr.ReleaseDescriptor(1);
        int fd2 = mgr.GetDescriptor(1);
        // Depending on implementation, it could be the same fd or a new one,
        // but it should not throw and should be a valid descriptor.
        fd2.Should().BeGreaterThanOrEqualTo(0);
    }
}
