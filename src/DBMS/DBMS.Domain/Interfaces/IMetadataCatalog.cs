namespace DBMS.Domain.Interfaces;
using System.Collections.Generic;
using DBMS.Domain.Models;

public interface IMetadataCatalog
{
    TableMeta? GetTableMeta(string name);
    IndexMeta? GetIndexMeta(int id);
    void UpdateMeta(CatalogObject obj);
    void DeleteMeta(int id);
}
