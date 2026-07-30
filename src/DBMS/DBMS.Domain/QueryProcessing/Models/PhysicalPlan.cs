using System;
using System.Collections.Generic;

namespace DBMS.Domain.QueryProcessing.Models;

public class PhysicalPlan
{
    public List<Operator> Operators { get; set; } = new();
    public Operator? Root { get; set; }
}
