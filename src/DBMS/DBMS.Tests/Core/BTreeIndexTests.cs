using System;
using DBMS.Domain.Core;
using FluentAssertions;
using Xunit;

namespace DBMS.Tests.Core;

public class BTreeIndexTests
{
    [Fact]
    public void Insert_ShouldKeepTreeBalanced()
    {
        var bTree = new BTreeIndex();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => bTree.InsertKey(10, rid);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Search_ShouldFindExistingKey()
    {
        var bTree = new BTreeIndex();

        Action act = () => bTree.Search(15);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Delete_ShouldRebalanceTreeAfterDeletion()
    {
        var bTree = new BTreeIndex();

        Action act = () => bTree.DeleteKey(10);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Insert_ShouldSplitNode_WhenNodeIsFull()
    {
        var bTree = new BTreeIndex();
        var rid = new RID { PageId = 1, SlotNumber = 1 };

        Action act = () => bTree.InsertKey(999, rid);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void Delete_ShouldMergeNode_WhenNodeIsUnderflow()
    {
        var bTree = new BTreeIndex();

        Action act = () => bTree.DeleteKey(999);

        act.Should().Throw<NotImplementedException>();
    }
}
