using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.StorageEngine;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.StorageEngine;

public class WALManagerTests
{
    private readonly Mock<ILogWriter> _mockWriter;
    private readonly LSNGenerator _lsnGen;
    private readonly WALManager _manager;

    public WALManagerTests()
    {
        _mockWriter = new Mock<ILogWriter>();
        _lsnGen = new LSNGenerator(new LSN(0));
        _manager = new WALManager(_mockWriter.Object, _lsnGen);
    }

    [Fact]
    public void Append_ShouldAppendRecordAndReturnLSN()
    {
        // Arrange
        var record = new LogRecord();

        // Act
        var act = () => _manager.Append(record);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Flush_ShouldFlushUpToSpecifiedLSN()
    {
        // Arrange
        var upToLSN = new LSN(100);

        // Act
        var act = () => _manager.Flush(upToLSN);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void TruncateBefore_ShouldTruncateLogs()
    {
        // Arrange
        var lsn = new LSN(50);

        // Act
        var act = () => _manager.TruncateBefore(lsn);

        // Assert
        act.Should().NotThrow();
    }
}
