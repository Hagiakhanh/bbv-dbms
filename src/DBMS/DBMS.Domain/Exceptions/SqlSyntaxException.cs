namespace DBMS.Domain.Exceptions;
using System;
using DBMS.Domain.Models;

public class SqlSyntaxException : SqlException
{
    public int Line { get; }
    public int Column { get; }

    public SqlSyntaxException(int line, int column) : base($"Syntax error at line {line}, column {column}")
    {
        Line = line;
        Column = column;
    }
}
