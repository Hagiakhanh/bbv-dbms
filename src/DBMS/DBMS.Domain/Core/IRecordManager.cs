using System.Collections.Generic;

namespace DBMS.Domain.Core;

public interface IRecordManager
{
    void CascadeAction(List<Row> referencedRows);
}
