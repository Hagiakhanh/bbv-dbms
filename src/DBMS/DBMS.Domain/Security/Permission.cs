using System;
using System.Collections.Generic;

namespace DBMS.Domain.Security;

public class Permission
{
    public string Action { get; set; }
    public int ObjectId { get; set; }
}
