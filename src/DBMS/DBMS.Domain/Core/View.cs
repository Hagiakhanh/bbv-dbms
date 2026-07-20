using System;

namespace DBMS.Domain.Core;

public class View
{
    public int ViewId { get; private set; }
    public string Name { get; private set; }
    public string QueryDefinition { get; private set; }

    public View(string name, string queryDefinition = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("View name cannot be empty", nameof(name));
        Name = name;
        QueryDefinition = queryDefinition;
    }

    public object Compile()
    {
        throw new NotImplementedException();
    }
}
