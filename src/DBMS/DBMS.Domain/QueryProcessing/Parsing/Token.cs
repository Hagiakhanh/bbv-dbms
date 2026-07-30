using System;

namespace DBMS.Domain.QueryProcessing.Parsing;

public enum TokenKind
{
    Keyword,
    Identifier,
    Literal,
    Operator,
    Punctuation,
    EOF
}

public class Token
{
    public TokenKind Kind { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Position { get; set; }

    public Token() { }

    public Token(TokenKind kind, string value, int position = 0)
    {
        Kind = kind;
        Value = value;
        Position = position;
    }
}
