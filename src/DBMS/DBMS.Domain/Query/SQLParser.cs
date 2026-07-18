using System;
using System.Collections.Generic;

namespace DBMS.Domain.Query;

public class SQLParser
{
    public object Parse(string sql)
    {
        throw new NotImplementedException();
    }

    private object[] Tokenize(string sql)
    {
        throw new NotImplementedException();
    }

    private object BuildAST(object[] tokens)
    {
        throw new NotImplementedException();
    }
}
