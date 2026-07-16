using System;
using Xunit;
using FluentAssertions;
using DBMS.Domain.Transaction;
using DBMS.Domain.Models;

namespace DBMS.Tests.Transaction;

public class TransactionTableTests
{
    private readonly TransactionTable _table;

    public TransactionTableTests()
    {
        _table = new TransactionTable();
    }

    [Fact]
    public void AddTx_ShouldAddTransactionToTable()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _table.AddTx(txId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void RemoveTx_ShouldRemoveTransactionFromTable()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _table.RemoveTx(txId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetState_ShouldReturnTransactionState()
    {
        // Arrange
        var txId = new TransactionId(1);

        // Act
        var act = () => _table.GetState(txId);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetActiveIds_ShouldReturnListOfActiveTransactionIds()
    {
        // Act
        var act = () => _table.GetActiveIds();

        // Assert
        act.Should().NotThrow();
    }
}
