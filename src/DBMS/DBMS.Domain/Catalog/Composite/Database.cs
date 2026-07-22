using System;
using System.Collections.Generic;
using DBMS.Domain.Storage;
using DBMS.Domain.Catalog.Iterator;

namespace DBMS.Domain.Catalog.Composite;

public class Database : ICatalogComposite, IIterableCatalog
{
    private readonly List<Schema> _schemas;

    public int DatabaseId { get; private set; }
    public string Name { get; private set; }
    public string Owner { get; private set; }
    public IReadOnlyList<Schema> Schemas => _schemas;
    
    public IReadOnlyCollection<ICatalogComponent> Children => _schemas;

    public ICatalogIterator CreateIterator()
    {
        throw new NotImplementedException();
    }

    public Database(int id, string name, string owner)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be empty.", nameof(name));

        DatabaseId = id;
        Name = name;
        Owner = owner;
        _schemas = new List<Schema>();
    }

    public Database()
    {
        _schemas = new List<Schema>();
    }

    public Schema CreateSchema(string name)
    {
        throw new NotImplementedException();
    }

    public void DropSchema(string name)
    {
        throw new NotImplementedException();
    }

    public Schema GetSchema(string name)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<Schema> GetSchemas()
    {
        throw new NotImplementedException();
    }

    public void Backup(string path, IFileManager fileManager)
    {
        throw new NotImplementedException();
    }

    public void Restore(string path, IFileManager fileManager)
    {
        throw new NotImplementedException();
    }
}

