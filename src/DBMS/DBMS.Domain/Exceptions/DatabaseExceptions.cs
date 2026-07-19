using System;

namespace DBMS.Domain.Exceptions;

public class InvalidNameException : Exception
{
    public InvalidNameException(string message) : base(message) { }
}

public class DuplicateNameException : Exception
{
    public DuplicateNameException(string message) : base(message) { }
}

public class DatabaseInUseException : Exception
{
    public DatabaseInUseException(string message) : base(message) { }
}

public class DuplicateSchemaException : Exception
{
    public DuplicateSchemaException(string message) : base(message) { }
}

public class SchemaNotFoundException : Exception
{
    public SchemaNotFoundException(string message) : base(message) { }
}
