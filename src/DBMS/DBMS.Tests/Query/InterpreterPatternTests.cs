using System;
using DBMS.Domain.Catalog;
using DBMS.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace DBMS.Tests.Query;

public class InterpreterPatternTests
{
    [Fact]
    public void Token_ShouldStoreKindAndValueCorrectly()
    {
        var token = new Token(TokenKind.Keyword, "SELECT", 0);

        token.Kind.Should().Be(TokenKind.Keyword);
        token.Value.Should().Be("SELECT");
        token.Position.Should().Be(0);
    }

    [Fact]
    public void Lexer_Tokenize_ShouldThrowNotImplementedException()
    {
        var lexer = new Lexer();

        Action act = () => lexer.Tokenize("SELECT * FROM users");

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ASTNode_SelectNode_ShouldInitializeCorrectNodeType()
    {
        var node = new SelectNode();

        node.NodeType.Should().Be(ASTNodeType.Select);
        node.Columns.Should().BeEmpty();
        node.FromClause.Should().BeNull();
        node.WhereClause.Should().BeNull();
    }

    [Fact]
    public void ASTNode_IdentifierNode_ShouldInitializeCorrectName()
    {
        var node = new IdentifierNode("users");

        node.NodeType.Should().Be(ASTNodeType.Identifier);
        node.Name.Should().Be("users");
    }

    [Fact]
    public void ASTNode_LiteralNode_ShouldInitializeCorrectValue()
    {
        var node = new LiteralNode(42, DBMS.Domain.Core.DataTypeEnum.INT);

        node.NodeType.Should().Be(ASTNodeType.Literal);
        node.Value.Should().Be(42);
        node.DataType.Should().Be(DBMS.Domain.Core.DataTypeEnum.INT);
    }

    [Fact]
    public void ASTNode_BinaryExpressionNode_ShouldInitializeOperands()
    {
        var left = new IdentifierNode("age");
        var right = new LiteralNode(18, DBMS.Domain.Core.DataTypeEnum.INT);
        var binaryNode = new BinaryExpressionNode
        {
            Operator = ">",
            Left = left,
            Right = right
        };

        binaryNode.NodeType.Should().Be(ASTNodeType.BinaryExpression);
        binaryNode.Operator.Should().Be(">");
        binaryNode.Left.Should().Be(left);
        binaryNode.Right.Should().Be(right);
    }

    [Fact]
    public void ASTNode_Interpret_ShouldThrowNotImplementedException()
    {
        ASTNode node = new IdentifierNode("id");

        Action act = () => node.Interpret(new object());

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void AST_ShouldInitializeWithRootNode()
    {
        var rootNode = new SelectNode();
        var ast = new AST(rootNode);

        ast.Root.Should().Be(rootNode);
    }

    [Fact]
    public void SemanticAnalyzer_BindWithNullAST_ShouldThrowArgumentNullException()
    {
        var catalogMock = new Mock<ICatalogManager>();
        var analyzer = new SemanticAnalyzer(catalogMock.Object);

        Action act = () => analyzer.Bind((AST)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void QueryExecutor_ExecuteWithRuntimeContext_ShouldThrowNotImplementedException()
    {
        var executor = new QueryExecutor();
        var plan = new PhysicalPlan();
        var ctx = new RuntimeContext { TransactionId = 1, SessionId = "s1" };

        Action act = () => executor.Execute(plan, ctx);

        act.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public void ResultCursor_MoveNext_ShouldThrowNotImplementedException()
    {
        var cursor = new ResultCursor();

        Action act = () => cursor.MoveNext();

        act.Should().Throw<NotImplementedException>();
    }
}
