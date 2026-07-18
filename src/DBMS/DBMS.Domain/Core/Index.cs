using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public abstract class Index
{
    public int IndexId { get; set; }
    public string Name { get; set; }
    public List<Column> Columns { get; set; }

    public abstract RID Search(object key);
    public abstract void InsertKey(object key, RID rid);
    public abstract void DeleteKey(object key);
}
