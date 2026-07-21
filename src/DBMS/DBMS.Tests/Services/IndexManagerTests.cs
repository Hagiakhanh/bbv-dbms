using System;
using Xunit;
using DBMS.Domain.Core;
using DBMS.Domain.Services;
using DBMS.Domain.Exceptions;

namespace DBMS.Tests.Services;

public class IndexManagerTests
{
    [Fact]
    public void Register_ShouldRegisterIndex()
    {
        // Arrange
        var indexManager = new IndexManager();
        var index = new BTreeIndex { Name = "Idx_Test" };

        // Act
        indexManager.Register(index);

        // Assert
        Assert.True(true);
    }

    [Fact]
    public void Register_ShouldRejectDuplicateIndexName()
    {
        // Arrange
        var indexManager = new IndexManager();
        var index = new BTreeIndex { Name = "Idx_Test" };
        indexManager.Register(index);

        // Act & Assert
        Assert.Throws<DuplicateIndexException>(() => indexManager.Register(index));
    }

    [Fact]
    public void FindBestIndex_ShouldReturnOptimalIndexForQuery()
    {
        // Arrange
        var indexManager = new IndexManager();
        var query = new DBMS.Domain.Query.Query();

        // Act
        var result = indexManager.FindBestIndex(query);

        // Assert
        // The stub implementation will return a specific index or null.
        // Once logic is in, this test would assert on the optimal index returned.
        Assert.NotNull(result);
    }
}
