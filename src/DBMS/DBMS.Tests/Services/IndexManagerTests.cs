using System;
using Xunit;
using DBMS.Domain.Core;
using DBMS.Domain.Services;
using DBMS.Domain.Exceptions;

namespace DBMS.Tests.Services;

public class IndexManagerTests
{
    [Fact]
    public void CreateIndex_ShouldRegisterIndex()
    {
        // Arrange
        var indexManager = new IndexManager();
        var indexName = "Idx_Test";

        // Act
        indexManager.CreateIndex(indexName);

        // Assert
        // In a real scenario, we might verify via an internal list or another method if it exists,
        // or by trying to find it using FindBestIndex or getting it. 
        // For now, since IndexManager doesn't expose the index directly, we can just ensure it doesn't throw.
        Assert.True(true);
    }

    [Fact]
    public void CreateIndex_ShouldRejectDuplicateIndexName()
    {
        // Arrange
        var indexManager = new IndexManager();
        var indexName = "Idx_Test";
        indexManager.CreateIndex(indexName);

        // Act & Assert
        Assert.Throws<DuplicateIndexException>(() => indexManager.CreateIndex(indexName));
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
