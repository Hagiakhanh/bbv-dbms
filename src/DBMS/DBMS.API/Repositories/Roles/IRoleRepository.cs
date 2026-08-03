using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.Domain.Security;

namespace DBMS.API.Repositories.Roles;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int roleId);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int roleId);
}
