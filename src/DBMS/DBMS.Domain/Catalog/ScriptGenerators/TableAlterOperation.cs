namespace DBMS.Domain.Catalog.ScriptGenerators;

public enum TableAlterType
{
    RENAME_TABLE,
    ADD_COLUMN,
    DROP_COLUMN,
    ALTER_COLUMN,
    ADD_CONSTRAINT,
    DROP_CONSTRAINT,
    ADD_INDEX,
    DROP_INDEX
}

public class TableAlterOperation
{
    public TableAlterType Type { get; set; }
    public object Definition { get; set; } = string.Empty;
}
