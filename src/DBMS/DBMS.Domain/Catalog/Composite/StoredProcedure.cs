using System;
using System.Collections.Generic;

namespace DBMS.Domain.Catalog.Composite;

public class StoredProcedure : ICatalogComponent
{
    public string Name { get; private set; }
    
    private readonly List<Column> _parameters = new();
    public IReadOnlyCollection<Column> Parameters => _parameters.AsReadOnly();
    
    public string Body { get; private set; }

    public StoredProcedure(string name, string body = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Procedure name cannot be empty", nameof(name));
        Name = name;
        Body = body;
    }

    public void Compile()
    {
        throw new NotImplementedException();
    }

    public object Execute(object[] args)
    {
        throw new NotImplementedException();
    }
}


