using System;
using System.Collections.Generic;
using DBMS.Domain.Core;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Core;

public class PartitionTests
{
    [Fact]
    public void InsertRecord_ShouldRouteRecordToCorrectPartition()
    {
        // Arrange
        
        // Act

        // Assert
    }

    [Fact]
    public void InsertRecord_ShouldRejectInvalidPartitionKey()
    {
        // Arrange
        
        // Act

        // Assert
    }
}
