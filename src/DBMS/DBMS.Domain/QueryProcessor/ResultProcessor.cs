namespace DBMS.Domain.QueryProcessor;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;
using DBMS.Domain.Transaction;

public class ResultProcessor
{
    public ResultSet Format(ResultCursor cursor)
    {
        throw new System.NotImplementedException();
    }
    public byte[] Serialize(Tuple row)
    {
        throw new System.NotImplementedException();
    }
    public Page<Tuple> Paginate(ResultCursor cursor, int pageSize)
    {
        throw new System.NotImplementedException();
    }
}
