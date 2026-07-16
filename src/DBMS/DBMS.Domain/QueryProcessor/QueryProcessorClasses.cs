namespace DBMS.Domain.QueryProcessor;

using DBMS.Domain.Models;
using DBMS.Domain.DatabaseObjectManagement;

public class SqlParser
{
    public string SqlText { get; set; }
    public ASTNode Ast { get; set; }

    public void Tokenize() => throw new System.NotImplementedException();
    public void Parse() => throw new System.NotImplementedException();
    public ASTNode BuildAST() => throw new System.NotImplementedException();
}

public class QueryValidator
{
    public ASTNode Ast { get; set; }
    public MetadataCatalog Catalog { get; set; }

    public void ValidateSyntax() => throw new System.NotImplementedException();
    public void ValidateSemantics() => throw new System.NotImplementedException();
    public void CheckPrivileges() => throw new System.NotImplementedException();
}

public class QueryOptimizer
{
    public ASTNode ValidatedAst { get; set; }
    public CostModel CostModel { get; set; }

    public void GenerateLogicalPlan() => throw new System.NotImplementedException();
    public ExecutionPlan GeneratePhysicalPlan() => throw new System.NotImplementedException();
    public ExecutionPlan Optimize() => throw new System.NotImplementedException();
}

public class ExecutionEngine
{
    public ExecutionPlan Plan { get; set; }
    public TransactionContext TxContext { get; set; }

    public ResultSet Execute() => throw new System.NotImplementedException();
    public void InitPhysicalOperators() => throw new System.NotImplementedException();
    public Tuple GetNextTuple() => throw new System.NotImplementedException();
}

public class ResultProcessor
{
    public TupleBuffer OutputBuffer { get; set; }
    public ResultSet ResultSet { get; set; }

    public void FormatTuple() => throw new System.NotImplementedException();
    public void Serialize() => throw new System.NotImplementedException();
    public void GetCursor() => throw new System.NotImplementedException();
}
