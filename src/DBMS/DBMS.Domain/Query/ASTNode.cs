using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public enum ASTNodeType
{
    Select,
    Identifier,
    Literal,
    BinaryExpression,
    Root
}

public interface IASTVisitor
{
    void Visit(ASTNode node);
    void Visit(SelectNode node);
    void Visit(IdentifierNode node);
    void Visit(LiteralNode node);
    void Visit(BinaryExpressionNode node);
}

public abstract class ASTNode
{
    public ASTNodeType NodeType { get; protected set; }
    public IReadOnlyList<ASTNode> Children { get; protected set; } = Array.Empty<ASTNode>();

    public virtual void Accept(IASTVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public virtual object Interpret(object context)
    {
        throw new NotImplementedException();
    }
}

public class SelectNode : ASTNode
{
    public IReadOnlyList<ASTNode> Columns { get; set; } = Array.Empty<ASTNode>();
    public ASTNode? FromClause { get; set; }
    public ASTNode? WhereClause { get; set; }

    public SelectNode()
    {
        NodeType = ASTNodeType.Select;
    }

    public override void Accept(IASTVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public override object Interpret(object context)
    {
        throw new NotImplementedException();
    }
}

public class IdentifierNode : ASTNode
{
    public string Name { get; set; } = string.Empty;

    public IdentifierNode()
    {
        NodeType = ASTNodeType.Identifier;
    }

    public IdentifierNode(string name) : this()
    {
        Name = name;
    }

    public override void Accept(IASTVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public override object Interpret(object context)
    {
        throw new NotImplementedException();
    }
}

public class LiteralNode : ASTNode
{
    public object? Value { get; set; }
    public DataTypeEnum DataType { get; set; }

    public LiteralNode()
    {
        NodeType = ASTNodeType.Literal;
    }

    public LiteralNode(object? value, DataTypeEnum dataType = DataTypeEnum.VARCHAR) : this()
    {
        Value = value;
        DataType = dataType;
    }

    public override void Accept(IASTVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public override object Interpret(object context)
    {
        throw new NotImplementedException();
    }
}

public class BinaryExpressionNode : ASTNode
{
    public string Operator { get; set; } = string.Empty;
    public ASTNode? Left { get; set; }
    public ASTNode? Right { get; set; }

    public BinaryExpressionNode()
    {
        NodeType = ASTNodeType.BinaryExpression;
    }

    public override void Accept(IASTVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public override object Interpret(object context)
    {
        throw new NotImplementedException();
    }
}
