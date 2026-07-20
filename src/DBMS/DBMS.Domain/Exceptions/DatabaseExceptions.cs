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

public class PermissionDeniedException : Exception
{
    public PermissionDeniedException(string message) : base(message) { }
}

public class DatabaseContainsSchemasException : Exception
{
    public DatabaseContainsSchemasException(string message) : base(message) { }
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

public class DuplicateTableException : Exception
{
    public DuplicateTableException(string message) : base(message) { }
}

public class TableNotFoundException : Exception
{
    public TableNotFoundException(string message) : base(message) { }
}

public class ForeignKeyReferenceException : Exception
{
    public ForeignKeyReferenceException(string message) : base(message) { }
}

public class DuplicateColumnException : Exception
{
    public DuplicateColumnException(string message) : base(message) { }
}

public class ColumnReferencedByConstraintException : Exception
{
    public ColumnReferencedByConstraintException(string message) : base(message) { }
}

public class CatalogException : Exception
{
    public CatalogException(string message) : base(message) { }
}
