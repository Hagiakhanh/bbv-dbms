using System;
using Xunit;
using FluentAssertions;
using Moq;
using DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

namespace DBMS.Tests.QueryProcessor;

public class QueryValidatorTests
{
    private readonly Mock<IMetadataCatalog> _mockCatalog;
    private readonly Mock<IAuthorizationManager> _mockAuthz;
    private readonly QueryValidator _validator;

    public QueryValidatorTests()
    {
        _mockCatalog = new Mock<IMetadataCatalog>();
        _mockAuthz = new Mock<IAuthorizationManager>();
        _validator = new QueryValidator(_mockCatalog.Object, _mockAuthz.Object);
    }

    [Fact]
    public void Validate_ShouldReturnValidatedAst()
    {
        // Arrange
        var ast = new ASTNode();

        // Act
        var act = () => _validator.Validate(ast);

        // Assert
        act.Should().NotThrow();
    }
}
