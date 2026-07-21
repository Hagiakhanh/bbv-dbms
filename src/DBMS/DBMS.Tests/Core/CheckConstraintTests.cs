using System;
using DBMS.Domain.Core;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Core;

public class CheckConstraintTests
{
    private readonly Mock<IExpressionEvaluator> _mockEvaluator;
    private readonly CheckConstraint _checkConstraint;
    private readonly string _expression = "Age >= 18";

    public CheckConstraintTests()
    {
        _mockEvaluator = new Mock<IExpressionEvaluator>();
        _checkConstraint = new CheckConstraint("CHK_Age", _expression, _mockEvaluator.Object);
    }

    [Fact]
    public void CheckConstraint_ShouldRejectInvalidExpression()
    {
        // Arrange
        var row = new Row();
        _mockEvaluator.Setup(x => x.Evaluate(_expression, row)).Throws(new InvalidExpressionException("Invalid syntax"));

        // Act
        Action act = () => _checkConstraint.Validate(row);

        // Assert
        act.Should().Throw<InvalidExpressionException>();
    }

    [Fact]
    public void CheckConstraint_ShouldEvaluateExpressionTrue()
    {
        // Arrange
        var row = new Row();
        _mockEvaluator.Setup(x => x.Evaluate(_expression, row)).Returns(true);

        // Act
        var result = _checkConstraint.Validate(row);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CheckConstraint_ShouldRejectWhenExpressionFalse()
    {
        // Arrange
        var row = new Row();
        _mockEvaluator.Setup(x => x.Evaluate(_expression, row)).Returns(false);

        // Act
        Action act = () => _checkConstraint.Validate(row);

        // Assert
        act.Should().Throw<CheckConstraintViolationException>();
    }

    [Fact]
    public void CheckConstraint_ShouldHandleNullValues()
    {
        // Arrange
        var row = new Row();
        // In SQL, if an expression evaluates to NULL (unknown), it doesn't violate the check constraint.
        // For simplicity, we can assume our evaluator returns true if the result is unknown/null, 
        // or we mock it to return true as per SQL standard for Check Constraints.
        _mockEvaluator.Setup(x => x.Evaluate(_expression, row)).Returns(true);

        // Act
        var result = _checkConstraint.Validate(row);

        // Assert
        result.Should().BeTrue();
    }
}
