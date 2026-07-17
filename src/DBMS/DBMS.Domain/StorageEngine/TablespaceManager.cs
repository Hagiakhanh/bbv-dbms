namespace DBMS.Domain.StorageEngine;
using System.Collections.Generic;
using DBMS.Domain.Models;
using DBMS.Domain.Interfaces;

public class TablespaceManager
{
    private Dictionary<int, Tablespace> tablespaces;
    private IMetadataCatalog catalog;
    public void CreateTablespace(string name, string path)
    {
        throw new System.NotImplementedException();
    }
    public Tablespace GetTablespace(int id)
    {
        throw new System.NotImplementedException();
    }
    public string MapDatabaseToFile(string db)
    {
        throw new System.NotImplementedException();
    }
}
