using System.Collections.Generic;

namespace DBMS.API.DTOs.Constraints
{
    public class CreateConstraintRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "PRIMARY_KEY"; // PRIMARY_KEY, FOREIGN_KEY, UNIQUE, CHECK
        public List<string> Columns { get; set; } = new();

        // Foreign Key fields
        public string? ReferenceTable { get; set; }
        public List<string>? ReferenceColumns { get; set; }
        public string OnDelete { get; set; } = "NO_ACTION"; // NO_ACTION, CASCADE, SET_NULL
        public string OnUpdate { get; set; } = "NO_ACTION";

        // Check Constraint expression
        public string? Expression { get; set; }
    }

    public class ConstraintDto
    {
        public int ConstraintId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string SchemaName { get; set; } = "dbo";
        public string DatabaseName { get; set; } = "master";
        public List<string> Columns { get; set; } = new();

        public string? ReferenceTable { get; set; }
        public List<string>? ReferenceColumns { get; set; }
        public string? OnDelete { get; set; }
        public string? OnUpdate { get; set; }
        public string? Expression { get; set; }
    }
}
