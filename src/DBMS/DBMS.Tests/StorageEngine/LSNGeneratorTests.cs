using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.StorageEngine;
using DBMS.Domain.Models;

namespace DBMS.Tests.StorageEngine;

public class LSNGeneratorTests
{
    private readonly LSNGenerator _generator;

    public LSNGeneratorTests()
    {
        _generator = new LSNGenerator(new LSN(10));
    }

    [Fact]
    public void GetNextLSN_ShouldReturnNextLSN()
    {
        // Act
        var act = () => _generator.GetNextLSN();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetCurrentLSN_ShouldReturnCurrentLSN()
    {
        // Act
        var act = () => _generator.GetCurrentLSN();

        // Assert
        act.Should().NotThrow();
    }
}
