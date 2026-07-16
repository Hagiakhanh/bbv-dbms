using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.StorageEngine;
using DBMS.Domain.Models;

namespace DBMS.Tests.StorageEngine;

public class LogWriterTests
{
    private readonly LogWriter _writer;

    public LogWriterTests()
    {
        var path = new AbsolutePath("/var/log/dbms/wal.log");
        _writer = new LogWriter(path);
    }

    [Fact]
    public void Write_ShouldWriteBytesToLog()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act
        var act = () => _writer.Write(bytes);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Fsync_ShouldSyncToDisk()
    {
        // Act
        var act = () => _writer.Fsync();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CurrentOffset_ShouldReturnLong()
    {
        // Act
        var act = () => _writer.CurrentOffset();

        // Assert
        act.Should().NotThrow();
    }
}
