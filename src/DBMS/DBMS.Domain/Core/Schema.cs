using System;
using System.Collections.Generic;
using System.Linq;

namespace DBMS.Domain.Core;

public class Schema
{
    public int SchemaId { get; private set; }
    public string Name { get; private set; }
    
    private readonly List<Table> _tables = new();
    public IReadOnlyCollection<Table> Tables => _tables.AsReadOnly();
    
    private readonly List<View> _views = new();
    public IReadOnlyCollection<View> Views => _views.AsReadOnly();
    
    private readonly List<StoredProcedure> _procedures = new();
    public IReadOnlyCollection<StoredProcedure> Procedures => _procedures.AsReadOnly();
    
    private readonly List<Sequence> _sequences = new();
    public IReadOnlyCollection<Sequence> Sequences => _sequences.AsReadOnly();

    public Schema(string name)
    {
        // if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Schema name cannot be empty", nameof(name));
        // Name = name;
        throw new NotImplementedException();
    }

    public void AddTable(Table table)
    {
        // if (table == null) throw new ArgumentNullException(nameof(table));
        // if (_tables.Any(t => t.Name == table.Name))
        //     throw new Exception($"Duplicate table name: {table.Name}");
        // _tables.Add(table);
        throw new NotImplementedException();
    }

    public void DropTable(string name)
    {
        // var table = _tables.FirstOrDefault(t => t.Name == name);
        // if (table == null) throw new Exception($"Table not found: {name}");
        // _tables.Remove(table);
        throw new NotImplementedException();
    }

    public Table GetTable(string name)
    {
        // return _tables.FirstOrDefault(t => t.Name == name);
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Table> GetTables()
    {
        // return Tables;
        throw new NotImplementedException();
    }

    public void CreateView(View view)
    {
        // if (view == null) throw new ArgumentNullException(nameof(view));
        // _views.Add(view);
        throw new NotImplementedException();
    }

    public void DropView(string name)
    {
        // _views.RemoveAll(v => v.Name == name);
        throw new NotImplementedException();
    }

    public void CreateProcedure(StoredProcedure proc)
    {
        // if (proc == null) throw new ArgumentNullException(nameof(proc));
        // _procedures.Add(proc);
        throw new NotImplementedException();
    }

    public void DropProcedure(string name)
    {
        // _procedures.RemoveAll(p => p.Name == name);
        throw new NotImplementedException();
    }

    public void CreateSequence(Sequence seq)
    {
        // if (seq == null) throw new ArgumentNullException(nameof(seq));
        // _sequences.Add(seq);
        throw new NotImplementedException();
    }
}
