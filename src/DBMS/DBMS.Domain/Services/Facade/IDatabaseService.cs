
namespace DBMS.Domain.Services.Facade;

public interface IDatabaseService
{
    Schema CreateSchema(Database database, string name);
    void DropSchema(Database database, string name, bool cascade);
    void RenameSchema(Database database, string oldName, string newName);
}
