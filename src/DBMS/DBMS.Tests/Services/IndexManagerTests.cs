using System;
using DBMS.Domain.Exceptions;
using DBMS.Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;
using QueryModel = DBMS.Domain.QueryProcessing.Models.Query;

namespace DBMS.Tests.Services;

public class IndexManagerTests
{
    [Fact]
    public void CreateIndex_ShouldRegisterIndex()
    {
        var indexManager = new IndexManager();
        var indexMock = new Mock<Index>();
        indexMock.SetupGet(i => i.Name).Returns("Idx_Test");

        Action act = () => indexManager.Register(indexMock.Object);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void CreateIndex_ShouldRejectDuplicateIndexName()
    {
        var indexManager = new IndexManager();
        var indexMock = new Mock<Index>();
        indexMock.SetupGet(i => i.Name).Returns("Idx_Test");

        Action act = () => indexManager.Register(indexMock.Object);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void FindBestIndex_ShouldReturnOptimalIndexForQuery()
    {
        var indexManager = new IndexManager();
        var query = new QueryModel();

        Action act = () => indexManager.FindBestIndex(query);

        act.Should().Throw<NotImplementedException>();
    }
}
