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
        throw new NotImplementedException("Test not implemented yet.");
    }

    [Fact]
    public void InsertRecord_ShouldRejectInvalidPartitionKey()
    {
        throw new NotImplementedException("Test not implemented yet.");
    }
}
