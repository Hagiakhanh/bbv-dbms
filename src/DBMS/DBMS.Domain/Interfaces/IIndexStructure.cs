namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IIndexStructure
{
    void Insert(Key key, RID rid);
    void Delete(Key key);
    RID? Search(Key key);
    IEnumerable<RID> RangeScan(Key fromKey, Key toKey);
}
