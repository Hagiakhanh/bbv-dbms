using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Optimization;

public class TableStats
{
    public int TableId { get; set; }
    public long RowCount { get; set; }
    public int PageCount { get; set; }
    public IReadOnlyDictionary<int, object> ColumnHistograms { get; set; } = new Dictionary<int, object>();
}
