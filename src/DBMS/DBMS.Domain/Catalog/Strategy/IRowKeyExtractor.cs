using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Strategy;

public interface IRowKeyExtractor
{
    object ExtractKey(Row row, List<Column> columns);
    bool HasNullValue(Row row, List<Column> columns);
}

