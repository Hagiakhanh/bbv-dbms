using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Strategy;

public class HashIndex : Index
{
    public override RID Search(object key)
    {
        throw new NotImplementedException();
    }

    public override void InsertKey(object key, RID rid)
    {
        throw new NotImplementedException();
    }

    public override void DeleteKey(object key)
    {
        throw new NotImplementedException();
    }
}

