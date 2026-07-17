using System;
using System.Collections.Generic;

namespace DBMS.Domain.Core;

public class Trigger
{
    public string Name { get; set; }
    public TriggerEvent Event { get; set; }
    public TriggerTiming Timing { get; set; }
    public string Body { get; set; }
}
