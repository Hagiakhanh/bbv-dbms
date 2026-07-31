namespace DBMS.API.DTOs.Columns
{
    public class CreateColumnRequest
    {
        public string? TableName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = "VARCHAR";
        public bool IsNullable { get; set; } = true;
        public string? DefaultValue { get; set; }
    }

    public class UpdateColumnRequest
    {
        public string? TableName { get; set; }
        public string? NewName { get; set; }
        public string? DataType { get; set; }
        public bool? IsNullable { get; set; }
        public string? DefaultValue { get; set; }
    }

    public class ColumnDto
    {
        public int ColumnId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsNullable { get; set; }
        public string? DefaultValue { get; set; }
    }
}
